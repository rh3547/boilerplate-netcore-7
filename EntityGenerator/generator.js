const fs = require('fs');
const fsPromises = require('fs').promises;
const path = require('path');

function handleFormSubmit(event) {
  event.preventDefault();

  const entityName = document.getElementById("entityName").value;
  const fields = Array.from(document.getElementsByClassName("field"));

  const fieldData = getFieldsData();

  // Generate the file using the provided data
  generateFile(entityName, fieldData, () => {
    const successMessage = document.getElementById("successMessage");
    successMessage.textContent = `Files have been generated successfully!`;
    successMessage.style.display = "block";

    setTimeout(() => {
      successMessage.style.display = "none";
    }, 5000);
  });
}

function addField() {
  const fieldContainer = document.createElement('div');
  fieldContainer.classList.add('field');

  const fieldTypeInput = document.createElement('input');
  fieldTypeInput.type = 'text';
  fieldTypeInput.placeholder = 'Field Type';
  fieldTypeInput.classList.add('field-type');
  fieldTypeInput.addEventListener('input', handleFieldInput);

  const fieldNameInput = document.createElement('input');
  fieldNameInput.type = 'text';
  fieldNameInput.placeholder = 'Field Name';
  fieldNameInput.classList.add('field-name');
  fieldNameInput.addEventListener('input', handleFieldInput);

  const nullableLabel = document.createElement('label');
  nullableLabel.textContent = 'Nullable';

  const nullableCheckbox = document.createElement('input');
  nullableCheckbox.type = 'checkbox';
  nullableCheckbox.checked = true;
  nullableCheckbox.classList.add('nullable-checkbox');

  nullableLabel.appendChild(nullableCheckbox);

  const removeButton = document.createElement('button');
  removeButton.type = 'button';
  removeButton.textContent = 'Remove';
  removeButton.addEventListener('click', () => {
    removeField(fieldContainer);
  });

  fieldContainer.appendChild(fieldTypeInput);
  fieldContainer.appendChild(fieldNameInput);
  fieldContainer.appendChild(nullableLabel);
  fieldContainer.appendChild(removeButton);

  document.querySelector('.field-container').appendChild(fieldContainer);
}

function removeField(fieldContainer) {
  fieldContainer.remove();
}

function handleFieldInput(event) {
  const input = event.target;
  const fieldContainer = input.parentElement;
  const isLastField = fieldContainer === document.querySelector('.field-container').lastElementChild;

  if (input.value.trim() !== '' && isLastField) {
    addField();
  }
}

async function loadConfig() {
  try {
    const configFile = await fsPromises.readFile('config.json', 'utf-8');
    const config = JSON.parse(configFile);
    return config;
  } catch (err) {
    console.error('Error loading the config file:', err);
    return null;
  }
}

function getFieldsData() {
  const fieldNames = document.querySelectorAll('.field-name');
  const fieldTypes = document.querySelectorAll('.field-type');
  const nullableCheckboxes = document.querySelectorAll('.nullable-checkbox');

  const fieldsData = [];

  for (let i = 0; i < fieldNames.length; i++) {
    if (fieldNames[i].value.trim() === '' || fieldTypes[i].value.trim() === '') {
      continue;
    }

    fieldsData.push({
      fieldName: fieldNames[i].value ?? `field${i + 1}`,
      fieldType: fieldTypes[i].value ?? "string",
      nullable: nullableCheckboxes[i].checked,
    });
  }

  return fieldsData;
}

async function generateFile(entityName, fieldData, onSuccess) {
  entityName = `${entityName.charAt(0).toUpperCase()}${entityName.slice(1)}`;

  const config = await loadConfig();
  if (!config) {
    console.error('Failed to load config. Aborting file generation.');
    return;
  }

  let basicFieldTypes = [
    "string",
    "int",
    "guid",
    "float",
    "short",
    "bool"
  ];

  let foreignKeyRegex = /{(.*)}/g;
  let listRegex = /[Ll]ist<(.+?)>.*/;

  // Entity file
  let entityFieldsCode = fieldData.map((field, index) => {
    let type = field.fieldType.replace(foreignKeyRegex, "");
    let isId = false;
    if (field.fieldName.toLowerCase() == "id") {
      isId = true;
    }
    return `${index == 0 || fieldData.length == 1 ? '' : '        '}public ${type}${field.nullable ? '?' : ''} ${field.fieldName} { get; set; }${field.nullable ? '' : (isId == true) ? ' = default!;' : ' = null!;'}`;
  }).join('\n');
  const entityTemplate = await loadTemplate('templates/class');
  const entityTargetDirectory = path.join(config.projectPath, config.classPath.replace(/{{entityName}}/g, entityName));
  const entityFileName = config.classFileName.replace(/{{entityName}}/g, entityName);
  const entityFileContent = entityTemplate.replace(/{{entityName}}/g, entityName).replace('{{fields}}', entityFieldsCode);
  await saveFile(entityTargetDirectory, entityFileName, entityFileContent);

  // DTO file
  let importsCode = fieldData.map((field, index) => {
    let type = field.fieldType.replace(foreignKeyRegex, "");
    if (!basicFieldTypes.includes(type.toLowerCase())) {
      return config.dtosImportNamespace.replace(/{{type}}/g, type);
    }
    return "";
  }).filter(x => x != "").join('\n');
  let singleDtoFieldsCode = fieldData.map((field, index) => {
    let type = field.fieldType.replace(foreignKeyRegex, "");
    let isId = false;
    if (field.fieldName.toLowerCase() == "id") {
      isId = true;
    }
    if (!basicFieldTypes.includes(type.toLowerCase())) {
      if (listRegex.test(type)) {
        let listType = listRegex.exec(type);
        if (listType && listType[1]) {
          type = `List<${listType[1]}SingleDTO>`;
        }
      }
      else {
        type = `${type}SingleDTO`;
      }
    }
    return `${index == 0 || fieldData.length == 1 ? '' : '        '}public ${type}${field.nullable ? '?' : ''} ${field.fieldName} { get; set; }${field.nullable ? '' : (isId == true) ? ' = default!;' : ' = null!;'}`;
  }).join('\n');
  let startIndex = 0;
  let searchDtoFieldsCode = fieldData.map((field, index) => {
    let type = field.fieldType.replace(foreignKeyRegex, "");
    if (field.fieldName.toLowerCase() == "id") {
      if (index == 0) startIndex++;
      return "";
    }
    if (!basicFieldTypes.includes(type.toLowerCase())) {
      if (index == 0) startIndex++;
      return "";
    }
    return `${index == startIndex ? '' : '        '}public List<FieldFilter>? ${field.fieldName} { get; set; }`;
  }).filter(x => x != "").join('\n');
  startIndex = 0;
  let addDtoFieldsCode = fieldData.map((field, index) => {
    let type = field.fieldType.replace(foreignKeyRegex, "");
    if (field.fieldName.toLowerCase() == "id") {
      if (index == 0) startIndex++;
      return "";
    }
    if (!basicFieldTypes.includes(type.toLowerCase())) {
      if (index == 0) startIndex++;
      return "";
    }
    return `${index == startIndex ? '' : '        '}public ${type}${field.nullable ? '?' : ''} ${field.fieldName} { get; set; }${field.nullable ? '' : ' = null!;'}`;
  }).filter(x => x != "").join('\n');
  const dtoTemplate = await loadTemplate('templates/dtos');
  const dtoTargetDirectory = path.join(config.projectPath, config.dtosPath.replace(/{{entityName}}/g, entityName));
  const dtoFileName = config.dtosFileName.replace(/{{entityName}}/g, entityName);
  const dtoFileContent = dtoTemplate.replace(/{{entityName}}/g, entityName)
    .replace('{{imports}}', importsCode)
    .replace('{{singleFields}}', singleDtoFieldsCode)
    .replace('{{singleFields}}', singleDtoFieldsCode)
    .replace('{{searchFields}}', searchDtoFieldsCode)
    .replace('{{addFields}}', addDtoFieldsCode)
    .replace('{{addFields}}', addDtoFieldsCode);
  await saveFile(dtoTargetDirectory, dtoFileName, dtoFileContent);

  // Mappings file
  const mappingsTemplate = await loadTemplate('templates/mapping_config');
  const mappingsTargetDirectory = path.join(config.projectPath, config.mappingConfigPath.replace(/{{entityName}}/g, entityName));
  const mappingsFileName = config.mappingConfigFileName.replace(/{{entityName}}/g, entityName);
  const mappingsFileContent = mappingsTemplate.replace(/{{entityName}}/g, entityName);
  await saveFile(mappingsTargetDirectory, mappingsFileName, mappingsFileContent);

  // Repository interface file
  const repositoryInterfaceTemplate = await loadTemplate('templates/repository_interface');
  const repositoryInterfaceTargetDirectory = path.join(config.projectPath, config.repositoryInterfacePath.replace(/{{entityName}}/g, entityName));
  const repositoryInterfaceFileName = config.repositoryInterfaceFileName.replace(/{{entityName}}/g, entityName);
  const repositoryInterfaceFileContent = repositoryInterfaceTemplate.replace(/{{entityName}}/g, entityName);
  await saveFile(repositoryInterfaceTargetDirectory, repositoryInterfaceFileName, repositoryInterfaceFileContent);

  // Service interface file
  const serviceInterfaceTemplate = await loadTemplate('templates/service_interface');
  const serviceInterfaceTargetDirectory = path.join(config.projectPath, config.serviceInterfacePath.replace(/{{entityName}}/g, entityName));
  const serviceInterfaceFileName = config.serviceInterfaceFileName.replace(/{{entityName}}/g, entityName);
  const serviceInterfaceFileContent = serviceInterfaceTemplate.replace(/{{entityName}}/g, entityName);
  await saveFile(serviceInterfaceTargetDirectory, serviceInterfaceFileName, serviceInterfaceFileContent);

  // EF configuration file
  let efConfigurationFieldsCode = fieldData.map((field, index) => {
    let buildText = "";
    if (field.fieldName.toLowerCase() == "id") {
      buildText = `builder.HasKey(b => b.${field.fieldName})`;
    }
    else if (!basicFieldTypes.includes(field.fieldType.replace(foreignKeyRegex, "").toLowerCase())) {
      let relationDetails = foreignKeyRegex.exec(field.fieldType);
      let foreignKeyParams = null;
      if (relationDetails && relationDetails[1]) {
        foreignKeyParams = relationDetails[1].split(",");
      }
      if (listRegex.test(field.fieldType)) {
        buildText = `builder.HasMany(b => b.${field.fieldName}).WithOne(${foreignKeyParams && foreignKeyParams[1] ? `b => b.${foreignKeyParams[1]}` : ''})${foreignKeyParams && foreignKeyParams[0] ? `.HasForeignKey(b => b.${foreignKeyParams[0]})` : ''}`;
      }
      else {
        buildText = `builder.HasOne(b => b.${field.fieldName}).WithOne()${foreignKeyParams && foreignKeyParams[0] ? `.HasForeignKey<${entityName}>(b => b.${foreignKeyParams[0]})` : ''}`;
      }
    }
    else {
      buildText = `builder.Property(b => b.${field.fieldName})${!field.nullable ? '.IsRequired()' : ''}`;
    }
    return `${index == 0 ? '' : '            '}${buildText};`;
  }).filter(x => x != "").join('\n');
  const efConfigurationTemplate = await loadTemplate('templates/ef_configuration');
  const efConfigurationTargetDirectory = path.join(config.projectPath, config.efConfigurationPath.replace(/{{entityName}}/g, entityName));
  const efConfigurationFileName = config.efConfigurationFileName.replace(/{{entityName}}/g, entityName);
  const efConfigurationFileContent = efConfigurationTemplate.replace(/{{entityName}}/g, entityName).replace('{{fields}}', efConfigurationFieldsCode);
  await saveFile(efConfigurationTargetDirectory, efConfigurationFileName, efConfigurationFileContent);

  // Repository file
  const repositoryTemplate = await loadTemplate('templates/repository');
  const repositoryTargetDirectory = path.join(config.projectPath, config.repositoryPath.replace(/{{entityName}}/g, entityName));
  const repositoryFileName = config.repositoryFileName.replace(/{{entityName}}/g, entityName);
  const repositoryFileContent = repositoryTemplate.replace(/{{entityName}}/g, entityName);
  await saveFile(repositoryTargetDirectory, repositoryFileName, repositoryFileContent);

  // Service file
  const serviceTemplate = await loadTemplate('templates/service');
  const serviceTargetDirectory = path.join(config.projectPath, config.servicePath.replace(/{{entityName}}/g, entityName));
  const serviceFileName = config.serviceFileName.replace(/{{entityName}}/g, entityName);
  const serviceFileContent = serviceTemplate.replace(/{{entityName}}/g, entityName);
  await saveFile(serviceTargetDirectory, serviceFileName, serviceFileContent);

  // Controller file
  const controllerTemplate = await loadTemplate('templates/controller');
  const controllerTargetDirectory = path.join(config.projectPath, config.controllerPath.replace(/{{entityName}}/g, entityName));
  const controllerFileName = config.controllerFileName.replace(/{{entityName}}/g, entityName);
  const controllerFileContent = controllerTemplate.replace(/{{entityName}}/g, entityName);
  await saveFile(controllerTargetDirectory, controllerFileName, controllerFileContent);

  onSuccess();
}

async function saveFile(targetDirectory, filename, fileContent) {
  // Ensure the target directory exists
  if (!fs.existsSync(targetDirectory)) {
    fs.mkdirSync(targetDirectory, { recursive: true });
  }

  // Save the generated file in the target directory
  const filePath = path.join(targetDirectory, `${filename}`);
  fs.writeFile(filePath, fileContent, 'utf-8', (err) => {
    if (err) {
      console.error('Error saving the file:', err);
    } else {
      console.log('File saved:', filePath);
    }
  });
}

async function loadTemplate(templateUrl) {
  try {
    const response = await fetch(templateUrl);
    if (!response.ok) {
      throw new Error(`HTTP error! status: ${response.status}`);
    }
    const template = await response.text();
    return template;
  } catch (error) {
    console.error(`Error fetching template: ${error}`);
    return '';
  }
}

document.getElementById("entityForm").addEventListener("submit", handleFormSubmit);
document.getElementById("addFieldButton").addEventListener("click", addField);

document.addEventListener('DOMContentLoaded', () => {
  addField(false);
});
function normalizeVersion(version) {
  // Check if version is already in the desired format
  if (/^\d+\.\d+\.\d+\.\d+$/.test(version)) {
    return version;
  }

  // Replace  any non-numeric or non-dot characters with a dot, then remove any consecutive dots
  let normalizedVersion = version
    .replace(/[^0-9.]+/g, ".")
    .replace(/\.{2,}/g, ".");

  // Ensure the version ends with a digit, if not, append ".1"
  if (!/\d$/.test(normalizedVersion)) {
    normalizedVersion += "1";
  }

  // Ensure the version has three dots, if not, append the necessary number of ".0"
  const dotCount = (normalizedVersion.match(/\./g) || []).length;
  if (dotCount < 3) {
    normalizedVersion += ".0".repeat(3 - dotCount);
  }

  return normalizedVersion;
}

const fs = require("fs").promises;

const version = process.argv[2];
const normalizedVersion = normalizeVersion(version);
const sha = process.argv[3];
const branch = process.argv[4];
const commits = process.argv[5];

const filePath = "./StreamMaster.API/AssemblyInfo.cs";

const content = `
using System.Reflection;

[assembly: AssemblyVersion("${normalizedVersion}")]
[assembly: AssemblyFileVersion("${normalizedVersion}")]
[assembly: AssemblyInformationalVersion("${version}.Sha.${sha}")]
`;

async function createOrUpdateAssemblyInfo() {
  try {
    // Write the content to AssemblyInfo.cs
    await fs.writeFile(filePath, content.trim(), "utf8");
    console.log("AssemblyInfo.cs has been created/updated successfully.");
  } catch (error) {
    console.error("Error creating/updating AssemblyInfo.cs:", error);
  }
}

createOrUpdateAssemblyInfo();

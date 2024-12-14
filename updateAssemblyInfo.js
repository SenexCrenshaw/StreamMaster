const fs = require("fs").promises;

function normalizeVersion(version) {
  // Extract numeric segments from the version string
  const segments = version.match(/\d+/g) ?? [];

  // If there are fewer than 4 segments, pad with zeros until there are 4
  while (segments.length < 4) {
    segments.push("0");
  }

  // If there are more than 4 segments, truncate the extras
  const chosenSegments = segments.slice(0, 4);

  // Join into a major.minor.build.revision format
  return chosenSegments.join(".");
}

const version = process.argv[2];
const sha = process.argv[3];
const branch = process.argv[4];
const commits = process.argv[5];

const normalizedVersion = normalizeVersion(version);
const filePath = "./StreamMaster.API/AssemblyInfo.cs";

const content = `
using System.Reflection;

[assembly: AssemblyVersion("${normalizedVersion}")]
[assembly: AssemblyFileVersion("${normalizedVersion}")]
[assembly: AssemblyInformationalVersion("${version}.Sha.${sha}")]
`;

async function createOrUpdateAssemblyInfo() {
  try {
    await fs.writeFile(filePath, content.trim(), "utf8");
    console.log("AssemblyInfo.cs has been created/updated successfully.");
  } catch (error) {
    console.error("Error creating/updating AssemblyInfo.cs:", error);
  }
}

createOrUpdateAssemblyInfo();

const fs = require("fs").promises;

const rawVersion = process.argv[2];
const sha = process.argv[3];
const branch = process.argv[4];
const commits = process.argv[5];

console.log(commits);

const filePath = "./StreamMaster.API/AssemblyInfo.cs";

function toSemVer(rawVersion) {
  const parts = rawVersion.match(/^(\d+\.\d+\.\d+)-.*\.(\d+)$/);
  if (!parts) {
    throw new Error(`Invalid version format: ${rawVersion}`);
  }
  return `${parts[1]}.${parts[2]}`; // Returns 'MAJOR.MINOR.PATCH.BUILD'
}

const version = toSemVer(rawVersion);

const content = `
using System.Reflection;

[assembly: AssemblyVersion("${version}")]
[assembly: AssemblyFileVersion("${version}")]
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

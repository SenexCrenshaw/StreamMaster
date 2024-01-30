const fs = require("fs").promises;

const version = process.argv[2];
const sha = process.argv[3];
const branch = process.argv[4];
const commits = process.argv[5];

console.log(commits);

const filePath = "./StreamMaster.API/AssemblyInfo.cs";
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

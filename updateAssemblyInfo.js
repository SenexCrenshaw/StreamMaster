const fs = require("fs").promises;

const version = process.argv[2]; // version from semantic-release
const filePath = "./StreamMaster.API/AssemblyInfo.cs";
const content = `
using System.Reflection;

[assembly: AssemblyVersion("${version}")]
[assembly: AssemblyFileVersion("${version}")]
[assembly: AssemblyInformationalVersion("${version}-versioning.2+Branch.versioning.Sha.20b37a4dc75ef87d51d9f92cece2ff764ce59788")]
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

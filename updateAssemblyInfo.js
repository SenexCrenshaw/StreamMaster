const fs = require("fs").promises;

function normalizeVersion(version) {
  // Extract numeric segments from the version string (ignoring branch name)
  const mainVersionMatch = version.match(/^([\d.]+)-[\w.]+/);
  const additionalSegmentMatch = version.match(/(\d+)$/);

  const mainSegments = mainVersionMatch ? mainVersionMatch[1].split(".") : [];
  const additionalSegment = additionalSegmentMatch
    ? additionalSegmentMatch[1]
    : "0";

  // Ensure we have at least 3 main segments, pad with zeros if necessary
  while (mainSegments.length < 3) {
    mainSegments.push("0");
  }

  // Append the additional segment as the revision
  mainSegments.push(additionalSegment);

  // Truncate extras if there are more than 4 segments
  const chosenSegments = mainSegments.slice(0, 4);

  // Join into a major.minor.build.revision format
  return chosenSegments.join(".");
}

const version = process.argv[2];
const sha = process.argv[3];
const branch = process.argv[4];

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

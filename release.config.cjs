/**
 * @type {import('semantic-release').GlobalConfig}
 */

module.exports = {
  branches: [
    // "+([0-9])?(.{+([0-9]),x}).x",
    "main",
    // {
    //   name: "beta",
    //   prerelease: true
    // },
    // {
    //   name: "alpha",
    //   prerelease: true
    // },
    {
      name: "!main",
      prerelease: true
    }
  ],
  ci: false,
  debug: false,
  dryRun: false,
  plugins: [
    [
      "@semantic-release/commit-analyzer",
      {
        preset: "angular",
        releaseRules: [
          { type: "docs", scope: "README", release: "patch" },
          { type: "refactor", release: "patch" },
          { type: "style", release: "patch" },
          { type: "add", release: "patch" }
        ],
        parserOpts: {
          noteKeywords: ["BREAKING CHANGE", "BREAKING CHANGES"]
        }
      }
    ],
    "@semantic-release/release-notes-generator",
    [
      "@semantic-release/changelog",
      {
        changelogFile: "CHANGELOG.md"
      }
    ],
    [
      "@semantic-release/exec",
      {
        verifyConditionsCmd: ":",
        publishCmd: [
          "node updateAssemblyInfo.js ${nextRelease.version} ${nextRelease.gitHead} ${nextRelease.channel}",
          "git add ./StreamMaster.API/AssemblyInfo.cs",
          'git diff-index --quiet HEAD || git commit -m "chore: update AssemblyInfo.cs to version ${nextRelease.version}"'
        ].join(" && ")
      }
    ],
    [
      "@semantic-release/git",
      {
        assets: ["CHANGELOG.md"]
      }
    ],
    [
      "@semantic-release/github",
      {
        assets: ["CHANGELOG.md"]
      }
    ]
  ]
};

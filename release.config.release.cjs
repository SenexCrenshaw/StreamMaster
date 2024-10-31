/**
 * @type {import('semantic-release').GlobalConfig}
 */

module.exports = {
  branches: [
    // "+([0-9])?(.{+([0-9]),x}).x",
    {
      channel: "main",
      name: "main",
      prerelease: false
    },
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
          { type: "versionminor", release: "minor" },
          { type: "style", release: "patch" },
          { type: "minor", release: "minor" },
          { scope: "no-release", release: false },
          { type: "update", release: "patch" }
        ],
        parserOpts: {
          noteKeywords: ["BREAKING CHANGE", "BREAKING CHANGES"]
        }
      }
    ],
    "@semantic-release/release-notes-generator",
    {
      preset: "angular",
      presetConfig: {
        types: [
          { type: "feat", section: "Features" },
          { type: "fix", section: "Bug Fixes" },
          { type: "docs", section: "Documentation" },
          { type: "style", section: "Code Style" },
          { type: "refactor", section: "Refactoring" },
          { type: "perf", section: "Performance" },
          { type: "test", section: "Tests" },
          { type: "chore", section: "Maintenance" }
        ]
      }
    },
    [
      ("@semantic-release/changelog",
      {
        changelogFile: "CHANGELOG.md"
      })
    ],
    [
      "@semantic-release/exec",
      {
        verifyConditionsCmd: ":",
        prepareCmd:
          "node updateAssemblyInfo.js ${nextRelease.version} ${nextRelease.gitHead} ${nextRelease.channel}",
        publishCmd: [
          "node updateAssemblyInfo.js ${nextRelease.version} ${nextRelease.gitHead} ${nextRelease.channel}",
          "git add ./StreamMaster.API/AssemblyInfo.cs",
          'git diff-index --quiet HEAD || git commit -m "chore: update AssemblyInfo.cs to version ${nextRelease.version}"'
        ].join(" && ")
      }
    ],
    [
      "@semantic-release/github",
      {
        assets: ["CHANGELOG.md"]
      }
    ],
    [
      "@semantic-release/git",
      {
        assets: ["CHANGELOG.md"]
      }
    ]
  ]
};

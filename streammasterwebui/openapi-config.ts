import { type ConfigFile } from '@rtk-query/codegen-openapi';

const config: ConfigFile = {
  apiFile: './src/store/emptyApi.ts',
  apiImport: 'emptySplitApi',
  exportName: 'iptvApi',
  flattenArg: true,
  hooks: true,
  outputFile: './src/store/iptvApi.ts',
  schemaFile: 'http://127.0.0.1:7095/swagger/v1/swagger.json',
  tag: true,
  // outputFiles: {
  //   './src/store/channel.ts': {
  //     filterEndpoints: [/channel/i],
  //   },
  //   './src/store/epg.ts': {
  //     filterEndpoints: [/epg/i],
  //   },
  //   './src/store/m3u.ts': {
  //     filterEndpoints: [/m3u/i],
  //   },
  //   './src/store/files.ts': {
  //     filterEndpoints: [/files/i],
  //   },
  // },
};

export default config;

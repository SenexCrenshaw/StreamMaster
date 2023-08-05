import { hubConnection } from '../../app/signalr';
import * as StreamMasterApi from '../iptvApi';

export const enhancedApi = StreamMasterApi.iptvApi.enhanceEndpoints({
  endpoints: {
    channelGroupsGetChannelGroup: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResult = (
            data: StreamMasterApi.ChannelGroupDto
          ) => {
            updateCachedData(
              (
              ) => {
                return data;
              }
            );
          };

          hubConnection.on(
            'ChannelGroupDtoUpdate',
            (data: StreamMasterApi.ChannelGroupDto) => {
              applyResult(data);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    epgFilesGetEpgFile: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResult = (
            data: StreamMasterApi.EpgFilesDto
          ) => {
            updateCachedData(
              (
              ) => {
                return data;
              }
            );
          };

          hubConnection.on(
            'EpgFilesDtoUpdate',
            (data: StreamMasterApi.EpgFilesDto) => {
              applyResult(data);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    epgFilesGetEpgFiles: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: StreamMasterApi.EpgFilesDto[]
          ) => {
            updateCachedData(
              (draft: StreamMasterApi.EpgFilesDto[]) => {
                data.forEach(function (cn) {
                  const foundIndex = draft.findIndex(
                    (x) => x.id === cn.id
                  );
                  if (foundIndex !== -1) {
                    draft[foundIndex] = cn;
                  } else {
                    draft.push(cn);
                  }
                });
                return draft;
              }
            );
          };

          hubConnection.on(
            'EPGFilesDtoesUpdate',
            (data: StreamMasterApi.EpgFilesDto[]) => {
              applyResults(data);
            }
          );


          const applyResult = (
            data: StreamMasterApi.EpgFilesDto
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.EpgFilesDto[]
              ) => {
                const foundIndex = draft.findIndex(
                  (x) => x.id === data.id
                );

                if (foundIndex === -1) {
                  draft.push(data);
                } else {
                  draft[foundIndex] = data;
                }

                return draft;
              }
            );
          };

          hubConnection.on(
            'EPGFilesDtoUpdate',
            (data: StreamMasterApi.EpgFilesDto) => {
              applyResult(data);
            }
          );

          const deleteResult = (

            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            id: any
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.EpgFilesDto[]
              ) => {
                return draft.filter((obj) => obj.id !== id);
              }
            );
          };

          hubConnection.on(
            'EPGFilesDtoDelete',
            (id: number) => {
              deleteResult(id);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    iconsGetIcon: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResult = (
            data: StreamMasterApi.IconFileDto
          ) => {
            updateCachedData(
              (
              ) => {
                return data;
              }
            );
          };

          hubConnection.on(
            'IconFileDtoUpdate',
            (data: StreamMasterApi.IconFileDto) => {
              applyResult(data);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    iconsGetIcons: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: StreamMasterApi.IconFileDto[]
          ) => {
            updateCachedData(
              (draft: StreamMasterApi.IconFileDto[]) => {
                data.forEach(function (cn) {
                  const foundIndex = draft.findIndex(
                    (x) => x.id === cn.id
                  );
                  if (foundIndex !== -1) {
                    draft[foundIndex] = cn;
                  } else {
                    draft.push(cn);
                  }
                });
                return draft;
              }
            );
          };

          hubConnection.on(
            'IconFileDtoesUpdate',
            (data: StreamMasterApi.IconFileDto[]) => {
              applyResults(data);
            }
          );


          const applyResult = (
            data: StreamMasterApi.IconFileDto
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.IconFileDto[]
              ) => {
                const foundIndex = draft.findIndex(
                  (x) => x.id === data.id
                );

                if (foundIndex === -1) {
                  draft.push(data);
                } else {
                  draft[foundIndex] = data;
                }

                return draft;
              }
            );
          };

          hubConnection.on(
            'IconFileDtoUpdate',
            (data: StreamMasterApi.IconFileDto) => {
              applyResult(data);
            }
          );

          const deleteResult = (

            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            id: any
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.IconFileDto[]
              ) => {
                return draft.filter((obj) => obj.id !== id);
              }
            );
          };

          hubConnection.on(
            'IconFileDtoDelete',
            (id: number) => {
              deleteResult(id);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    m3UFilesGetM3UFiles: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: StreamMasterApi.M3UFileDto[]
          ) => {
            updateCachedData(
              (draft: StreamMasterApi.M3UFileDto[]) => {
                data.forEach(function (cn) {
                  const foundIndex = draft.findIndex(
                    (x) => x.id === cn.id
                  );
                  if (foundIndex !== -1) {
                    draft[foundIndex] = cn;
                  } else {
                    draft.push(cn);
                  }
                });
                return draft;
              }
            );
          };

          hubConnection.on(
            'M3UFileDtosUpdate',
            (data: StreamMasterApi.M3UFileDto[]) => {
              applyResults(data);
            }
          );


          const applyResult = (
            data: StreamMasterApi.M3UFileDto
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.M3UFileDto[]
              ) => {
                const foundIndex = draft.findIndex(
                  (x) => x.id === data.id
                );

                if (foundIndex === -1) {
                  draft.push(data);
                } else {
                  draft[foundIndex] = data;
                }

                return draft;
              }
            );
          };

          hubConnection.on(
            'M3UFileDtoUpdate',
            (data: StreamMasterApi.M3UFileDto) => {
              applyResult(data);
            }
          );

          const deleteResult = (

            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            id: any
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.M3UFileDto[]
              ) => {
                return draft.filter((obj) => obj.id !== id);
              }
            );
          };

          hubConnection.on(
            'M3UFileDtoDelete',
            (id: number) => {
              deleteResult(id);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    schedulesDirectGetStationPreviews: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: StreamMasterApi.StationPreview[]
          ) => {
            updateCachedData(
              (draft: StreamMasterApi.StationPreview[]) => {
                data.forEach(function (cn) {
                  const foundIndex = draft.findIndex(
                    (x) => x.id === cn.id
                  );
                  if (foundIndex !== -1) {
                    draft[foundIndex] = cn;
                  } else {
                    draft.push(cn);
                  }
                });
                return draft;
              }
            );
          };

          hubConnection.on(
            'StationPreviewsUpdate',
            (data: StreamMasterApi.StationPreview[]) => {
              applyResults(data);
            }
          );


          const applyResult = (
            data: StreamMasterApi.StationPreview
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.StationPreview[]
              ) => {
                const foundIndex = draft.findIndex(
                  (x) => x.id === data.id
                );

                if (foundIndex === -1) {
                  draft.push(data);
                } else {
                  draft[foundIndex] = data;
                }

                return draft;
              }
            );
          };

          hubConnection.on(
            'StationPreviewUpdate',
            (data: StreamMasterApi.StationPreview) => {
              applyResult(data);
            }
          );

          const deleteResult = (

            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            id: any
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.StationPreview[]
              ) => {
                return draft.filter((obj) => obj.id !== id);
              }
            );
          };

          hubConnection.on(
            'StationPreviewDelete',
            (id: number) => {
              deleteResult(id);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    settingsGetQueueStatus: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: StreamMasterApi.TaskQueueStatusDto[]
          ) => {
            updateCachedData(
              (draft: StreamMasterApi.TaskQueueStatusDto[]) => {
                data.forEach(function (cn) {
                  const foundIndex = draft.findIndex(
                    (x) => x.id === cn.id
                  );
                  if (foundIndex !== -1) {
                    draft[foundIndex] = cn;
                  } else {
                    draft.push(cn);
                  }
                });
                return draft;
              }
            );
          };

          hubConnection.on(
            'TaskQueueStatusDtoesUpdate',
            (data: StreamMasterApi.TaskQueueStatusDto[]) => {
              applyResults(data);
            }
          );


          const applyResult = (
            data: StreamMasterApi.TaskQueueStatusDto
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.TaskQueueStatusDto[]
              ) => {
                const foundIndex = draft.findIndex(
                  (x) => x.id === data.id
                );

                if (foundIndex === -1) {
                  draft.push(data);
                } else {
                  draft[foundIndex] = data;
                }

                return draft;
              }
            );
          };

          hubConnection.on(
            'TaskQueueStatusDtoUpdate',
            (data: StreamMasterApi.TaskQueueStatusDto) => {
              applyResult(data);
            }
          );

          const deleteResult = (

            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            id: any
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.TaskQueueStatusDto[]
              ) => {
                return draft.filter((obj) => obj.id !== id);
              }
            );
          };

          hubConnection.on(
            'TaskQueueStatusDtoDelete',
            (id: number) => {
              deleteResult(id);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    settingsGetSetting: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResult = (
            data: StreamMasterApi.SettingDto
          ) => {
            updateCachedData(
              (
              ) => {
                return data;
              }
            );
          };

          hubConnection.on(
            'SettingDtoUpdate',
            (data: StreamMasterApi.SettingDto) => {
              applyResult(data);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    settingsGetSystemStatus: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResult = (
            data: StreamMasterApi.SystemStatus
          ) => {
            updateCachedData(
              (
              ) => {
                return data;
              }
            );
          };

          hubConnection.on(
            'SystemStatusUpdate',
            (data: StreamMasterApi.SystemStatus) => {
              applyResult(data);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    streamGroupsGetStreamGroup: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResult = (
            data: StreamMasterApi.StreamGroupDto
          ) => {
            updateCachedData(
              (
              ) => {
                return data;
              }
            );
          };

          hubConnection.on(
            'StreamGroupDtoUpdate',
            (data: StreamMasterApi.StreamGroupDto) => {
              applyResult(data);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    streamGroupsGetStreamGroupEpgForGuide: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResult = (
            data: StreamMasterApi.EpgGuide
          ) => {
            updateCachedData(
              (
              ) => {
                return data;
              }
            );
          };

          hubConnection.on(
            'EpgGuideUpdate',
            (data: StreamMasterApi.EpgGuide) => {
              applyResult(data);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    streamGroupsGetStreamGroups: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: StreamMasterApi.StreamGroupDto[]
          ) => {
            updateCachedData(
              (draft: StreamMasterApi.StreamGroupDto[]) => {
                data.forEach(function (cn) {
                  const foundIndex = draft.findIndex(
                    (x) => x.id === cn.id
                  );
                  if (foundIndex !== -1) {
                    draft[foundIndex] = cn;
                  } else {
                    draft.push(cn);
                  }
                });
                return draft;
              }
            );
          };

          hubConnection.on(
            'StreamGroupDtoesUpdate',
            (data: StreamMasterApi.StreamGroupDto[]) => {
              applyResults(data);
            }
          );


          const applyResult = (
            data: StreamMasterApi.StreamGroupDto
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.StreamGroupDto[]
              ) => {
                const foundIndex = draft.findIndex(
                  (x) => x.id === data.id
                );

                if (foundIndex === -1) {
                  draft.push(data);
                } else {
                  draft[foundIndex] = data;
                }

                return draft;
              }
            );
          };

          hubConnection.on(
            'StreamGroupDtoUpdate',
            (data: StreamMasterApi.StreamGroupDto) => {
              applyResult(data);
            }
          );

          const deleteResult = (

            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            id: any
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.StreamGroupDto[]
              ) => {
                return draft.filter((obj) => obj.id !== id);
              }
            );
          };

          hubConnection.on(
            'StreamGroupDtoDelete',
            (id: number) => {
              deleteResult(id);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
    videoStreamsGetChannelLogoDtos: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResults = (
            data: StreamMasterApi.ChannelLogoDto[]
          ) => {
            updateCachedData(
              (draft: StreamMasterApi.ChannelLogoDto[]) => {
                data.forEach(function (cn) {
                  const foundIndex = draft.findIndex(
                    (x) => x.logoUrl === cn.logoUrl
                  );
                  if (foundIndex !== -1) {
                    draft[foundIndex] = cn;
                  } else {
                    draft.push(cn);
                  }
                });
                return draft;
              }
            );
          };

          hubConnection.on(
            'ChannelLogoDtoesUpdate',
            (data: StreamMasterApi.ChannelLogoDto[]) => {
              applyResults(data);
            }
          );


          const applyResult = (
            data: StreamMasterApi.ChannelLogoDto
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.ChannelLogoDto[]
              ) => {
                const foundIndex = draft.findIndex(
                  (x) => x.logoUrl === data.logoUrl
                );

                if (foundIndex === -1) {
                  draft.push(data);
                } else {
                  draft[foundIndex] = data;
                }

                return draft;
              }
            );
          };

          hubConnection.on(
            'ChannelLogoDtoUpdate',
            (data: StreamMasterApi.ChannelLogoDto) => {
              applyResult(data);
            }
          );

          const deleteResult = (

            // eslint-disable-next-line @typescript-eslint/no-explicit-any
            logoUrl: any
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.ChannelLogoDto[]
              ) => {
                return draft.filter((obj) => obj.logoUrl !== logoUrl);
              }
            );
          };

          hubConnection.on(
            'ChannelLogoDtoDelete',
            (id: number) => {
              deleteResult(id);
            }
          );


        } catch { }

        await cacheEntryRemoved;
      }
    },
  }
});

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


                } catch {}

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


                } catch {}

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
              ( draft: StreamMasterApi.EpgFilesDto[]) => {
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


                } catch {}

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


                } catch {}

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
              ( draft: StreamMasterApi.IconFileDto[]) => {
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


                } catch {}

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
            data: StreamMasterApi.M3UFilesDto[]
          ) => {
            updateCachedData(
              ( draft: StreamMasterApi.M3UFilesDto[]) => {
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
            'M3UFilesDtosUpdate',
            (data: StreamMasterApi.M3UFilesDto[]) => {
              applyResults(data);
            }
          );


          const applyResult = (
            data: StreamMasterApi.M3UFilesDto
          ) => {
            updateCachedData(
              (
                draft: StreamMasterApi.M3UFilesDto[]
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
                        'M3UFilesDtoUpdate',
                        (data: StreamMasterApi.M3UFilesDto) => {
                            applyResult(data);
                        }
                    );

                    const deleteResult = (

                        // eslint-disable-next-line @typescript-eslint/no-explicit-any
                        id: any
                    ) => {
                        updateCachedData(
                            (
                                draft: StreamMasterApi.M3UFilesDto[]
                            ) => {
                              return draft.filter((obj) => obj.id !== id);
                            }
                        );
                    };

                    hubConnection.on(
                        'M3UFilesDtoDelete',
                        (id: number) => {
                            deleteResult(id);
                        }
                    );


                } catch {}

                await cacheEntryRemoved;
            }
        },
    settingsGetIsSystemReady: {
      async onCacheEntryAdded(
        arg,
        { updateCachedData, cacheDataLoaded, cacheEntryRemoved }
      ) {
        try {
          await cacheDataLoaded;

          const applyResult = (
            data: boolean
          ) => {
            updateCachedData(
              (
              ) => {
                return data;
        }
            );
                    };

                    hubConnection.on(
                        'booleanUpdate',
                        (data: boolean) => {
                            applyResult(data);
                        }
                    );


                } catch {}

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
              ( draft: StreamMasterApi.TaskQueueStatusDto[]) => {
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


                } catch {}

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


                } catch {}

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


                } catch {}

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


                } catch {}

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


                } catch {}

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
              ( draft: StreamMasterApi.StreamGroupDto[]) => {
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


                } catch {}

                await cacheEntryRemoved;
            }
        },
    }
});
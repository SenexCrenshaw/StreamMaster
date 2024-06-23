import StringEditor from '@components/inputs/StringEditor';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { UpdateVideoProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import useGetVideoProfiles from '@lib/smAPI/Profiles/useGetVideoProfiles';
import { UpdateVideoProfileRequest, VideoOutputProfileDto } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';
import CreateVideoProfileDialog from './CreateVideoProfileDialog';
import RemoveVideoProfileDialog from './RemoveVideoProfileDialog';

import { useVideoIsM3U8ColumnConfig } from './columns/useVideoIsM3U8ColumnConfig';
import { useVideoProfileCommandColumnConfig } from './columns/useVideoProfileCommandColumnConfig';
import { useVideoProfileParametersColumnConfig } from './columns/useVideoProfileParametersColumnConfig';
import { useVideoProfileTimeoutColumnConfig } from './columns/useVideoProfileTimeoutColumnConfig';

const StreamGroupVideoProfileDataSelector = () => {
  const id = 'StreamGroupVideoProfileDataSelector';
  const { data } = useGetVideoProfiles();
  const videoProfileCommandColumnConfig = useVideoProfileCommandColumnConfig({ width: 40 });
  const videoProfileParametersColumnConfig = useVideoProfileParametersColumnConfig({ width: 200 });
  const videoProfileTimeoutColumnConfig = useVideoProfileTimeoutColumnConfig({ width: 20 });
  const videoProfilIsM3U8ColumnConfig = useVideoIsM3U8ColumnConfig({ width: 20 });

  const actionTemplate = useCallback((rowData: VideoOutputProfileDto) => {
    return (
      <div className="flex justify-content-center align-items-center">
        <RemoveVideoProfileDialog videoOutputProfileDto={rowData} />
        {/* <StreamGroupDeleteDialog streamGroup={rowData} /> */}
        {/* <M3UFileRefreshDialog selectedFile={rowData} />
         <M3UFileRemoveDialog selectedFile={rowData} /> */}
        {/* <EPGFileEditDialog selectedFile={rowData} /> */}
      </div>
    );
  }, []);

  const update = useCallback((request: UpdateVideoProfileRequest) => {
    UpdateVideoProfile(request)
      .then((res) => {})
      .catch((error) => {
        console.error('error', error);
      })
      .finally();
  }, []);

  const nameTemplate = useCallback(
    (rowData: VideoOutputProfileDto) => {
      if (rowData.ProfileName.toLowerCase() === 'default') {
        return <div className="text-container pl-1">{rowData.ProfileName}</div>;
      }
      return (
        <StringEditor
          value={rowData.ProfileName}
          onSave={(e) => {
            if (e !== undefined) {
              const ret = { ProfileName: rowData.ProfileName, NewName: e } as UpdateVideoProfileRequest;
              update(ret);
            }
          }}
        />
      );
    },
    [update]
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      {
        bodyTemplate: nameTemplate,
        field: 'ProfileName',
        filter: true,
        sortable: true,
        width: 40
      },
      videoProfileCommandColumnConfig,
      videoProfileParametersColumnConfig,
      videoProfileTimeoutColumnConfig,
      videoProfilIsM3U8ColumnConfig,
      {
        align: 'center',
        bodyTemplate: actionTemplate,
        field: 'autoUpdate',
        fieldType: 'actions',
        width: 40
      }
    ],
    [
      actionTemplate,
      nameTemplate,
      videoProfilIsM3U8ColumnConfig,
      videoProfileCommandColumnConfig,
      videoProfileParametersColumnConfig,
      videoProfileTimeoutColumnConfig
    ]
  );

  return (
    <SMDataTable
      actionHeaderTemplate={<CreateVideoProfileDialog />}
      columns={columns}
      dataSource={data}
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Profiles"
      enableClick
      enableExport={false}
      headerName="Video Profiles"
      id={id}
      lazy
      onRowClick={(e: DataTableRowClickEvent) => {}}
      style={{ height: '30vh' }}
    />
  );
};

StreamGroupVideoProfileDataSelector.displayName = 'StreamGroupVideoProfileDataSelector';

export default memo(StreamGroupVideoProfileDataSelector);

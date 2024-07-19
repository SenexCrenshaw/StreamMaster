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
import { useVideoProfileCommandColumnConfig } from './columns/useVideoProfileCommandColumnConfig';
import { useVideoProfileParametersColumnConfig } from './columns/useVideoProfileParametersColumnConfig';

const StreamGroupVideoProfileDataSelector = () => {
  const id = 'StreamGroupVideoProfileDataSelector';
  const { data } = useGetVideoProfiles();
  const videoProfileCommandColumnConfig = useVideoProfileCommandColumnConfig({ width: 40 });
  const videoProfileParametersColumnConfig = useVideoProfileParametersColumnConfig({ width: 200 });

  const actionTemplate = useCallback((rowData: VideoOutputProfileDto) => {
    return (
      <div className="sm-center-stuff">
        <RemoveVideoProfileDialog videoOutputProfileDto={rowData} />
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
      if (
        rowData.ProfileName.toLowerCase() === 'default' ||
        rowData.ProfileName.toLowerCase() === 'defaultffmpeg' ||
        rowData.ProfileName.toLowerCase() === 'streammaster'
      ) {
        return <div className="text-container pl-1">{rowData.ProfileName}</div>;
      }
      return (
        <StringEditor
          value={rowData.ProfileName}
          onSave={(e) => {
            if (e !== undefined) {
              const ret = { NewName: e, ProfileName: rowData.ProfileName } as UpdateVideoProfileRequest;
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
        width: 60
      },
      videoProfileCommandColumnConfig,
      videoProfileParametersColumnConfig,
      // videoProfileTimeoutColumnConfig,
      {
        align: 'center',
        bodyTemplate: actionTemplate,
        field: 'autoUpdate',
        fieldType: 'actions',
        width: 20
      }
    ],
    [actionTemplate, nameTemplate, videoProfileCommandColumnConfig, videoProfileParametersColumnConfig]
  );

  // const rowClass = useCallback((data: DataTableRowData<any>) => {
  //   if (data === undefined) {
  //     return '';
  //   }

  //   const videoOutputProfileDto = data as unknown as VideoOutputProfileDto;

  //   if (videoOutputProfileDto.IsReadOnly === true || videoOutputProfileDto.ProfileName.toLowerCase() === 'defaultffmpeg') {
  //     return 'p-disabled';
  //   }
  //   return '';
  // }, []);

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
      // rowClass={rowClass}
      id={id}
      lazy
      onRowClick={(e: DataTableRowClickEvent) => {}}
      style={{ height: '30vh' }}
    />
  );
};

StreamGroupVideoProfileDataSelector.displayName = 'StreamGroupVideoProfileDataSelector';

export default memo(StreamGroupVideoProfileDataSelector);

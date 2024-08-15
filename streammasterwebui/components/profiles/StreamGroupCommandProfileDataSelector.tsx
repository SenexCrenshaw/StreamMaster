import StringEditor from '@components/inputs/StringEditor';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { UpdateCommandProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import useGetCommandProfiles from '@lib/smAPI/Profiles/useGetCommandProfiles';
import { CommandProfileDto, UpdateCommandProfileRequest } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';
import { useCommandProfileCommandColumnConfig } from './columns/useCommandProfileCommandColumnConfig';
import { useCommandProfileParametersColumnConfig } from './columns/useCommandProfileParametersColumnConfig';
import CreateCommandProfileDialog from './CreateCommandProfileDialog';
import RemoveCommandProfileDialog from './RemoveCommandProfileDialog';

const StreamGroupCommandProfileDataSelector = () => {
  const id = 'StreamGroupCommandProfileDataSelector';
  const { data } = useGetCommandProfiles();
  const CommandProfileCommandColumnConfig = useCommandProfileCommandColumnConfig({ width: 40 });
  const CommandProfileParametersColumnConfig = useCommandProfileParametersColumnConfig({ width: 200 });

  const actionTemplate = useCallback((rowData: CommandProfileDto) => {
    return (
      <div className="sm-center-stuff">
        <RemoveCommandProfileDialog commandProfileDto={rowData} />
      </div>
    );
  }, []);

  const update = useCallback((request: UpdateCommandProfileRequest) => {
    UpdateCommandProfile(request)
      .then((res) => {})
      .catch((error) => {
        console.error('error', error);
      })
      .finally();
  }, []);

  const nameTemplate = useCallback(
    (rowData: CommandProfileDto) => {
      if (
        rowData.IsReadOnly === true
        // rowData.ProfileName.toLowerCase() === 'none' ||
        // rowData.ProfileName.toLowerCase() === 'ffmpeg' ||
        // rowData.ProfileName.toLowerCase() === 'use sg' ||
        // rowData.ProfileName.toLowerCase() === 'streammaster'
      ) {
        return <div className="text-container pl-1">{rowData.ProfileName}</div>;
      }
      return (
        <StringEditor
          value={rowData.ProfileName}
          onSave={(e) => {
            if (e !== undefined) {
              const ret = { NewProfileName: e, ProfileName: rowData.ProfileName } as UpdateCommandProfileRequest;
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
        header: 'Profile Name',
        sortable: true,
        width: 60
      },
      CommandProfileCommandColumnConfig,
      CommandProfileParametersColumnConfig,
      // CommandProfileTimeoutColumnConfig,
      {
        align: 'center',
        bodyTemplate: actionTemplate,
        field: 'autoUpdate',
        fieldType: 'actions',
        width: 20
      }
    ],
    [actionTemplate, nameTemplate, CommandProfileCommandColumnConfig, CommandProfileParametersColumnConfig]
  );

  // const rowClass = useCallback((data: DataTableRowData<any>) => {
  //   if (data === undefined) {
  //     return '';
  //   }

  //   const videoOutputProfileDto = data as unknown as CommandProfileDto;

  //   if (videoOutputProfileDto.IsReadOnly === true || videoOutputProfileDto.ProfileName.toLowerCase() === 'defaultffmpeg') {
  //     return 'p-disabled';
  //   }
  //   return '';
  // }, []);

  return (
    <SMDataTable
      actionHeaderTemplate={<CreateCommandProfileDialog />}
      columns={columns}
      dataSource={data}
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Profiles"
      enableClick
      enableExport={false}
      headerName="Command Profiles"
      // rowClass={rowClass}
      id={id}
      dataKey="ProfileName"
      lazy
      onRowClick={(e: DataTableRowClickEvent) => {}}
      style={{ height: '30vh' }}
    />
  );
};

StreamGroupCommandProfileDataSelector.displayName = 'StreamGroupCommandProfileDataSelector';

export default memo(StreamGroupCommandProfileDataSelector);

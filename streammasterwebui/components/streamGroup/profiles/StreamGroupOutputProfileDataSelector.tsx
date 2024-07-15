import StringEditor from '@components/inputs/StringEditor';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';

import { UpdateOutputProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import useGetOutputProfiles from '@lib/smAPI/Profiles/useGetOutputProfiles';
import { OutputProfileDto, UpdateOutputProfileRequest } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';
import CreateFileProfileDialog from './CreateFileProfileDialog';
import RemoveOutputProfileDialog from './RemoveOutputProfileDialog';
import { useOutputProfileChannelNumberColumnConfig } from './columns/useOutputProfileChannelNumberColumnConfig';
import { useOutputProfileEPGIdColumnConfig } from './columns/useOutputProfileEPGIdColumnConfig';
import { useOutputProfileGroupColumnConfig } from './columns/useOutputProfileGroupColumnConfig';
import { useOutputProfileGroupTitleColumnConfig } from './columns/useOutputProfileGroupTitleColumnConfig';
import { useOutputProfileIconColumnConfig } from './columns/useOutputProfileIconColumnConfig';
import { useOutputProfileIdColumnConfig } from './columns/useOutputProfileIdColumnConfig';
import { useOutputProfileNameColumnConfig } from './columns/useOutputProfileNameColumnConfig';

const StreamGroupOutputProfileDataSelector = () => {
  const id = 'StreamGroupOutputProfileDataSelector';

  const { data } = useGetOutputProfiles();

  const nameColumnConfig = useOutputProfileNameColumnConfig({ width: 40 });
  const groupColumnConfig = useOutputProfileGroupColumnConfig({ width: 40 });
  const iDColumnConfig = useOutputProfileIdColumnConfig({ width: 40 });
  const groupTitleColumnConfig = useOutputProfileGroupTitleColumnConfig({ width: 40 });
  const channelNumberColumnConfig = useOutputProfileChannelNumberColumnConfig({ width: 40 });
  const epgIdColumnConfig = useOutputProfileEPGIdColumnConfig({ width: 40 });
  const iconColumnConfig = useOutputProfileIconColumnConfig({ width: 40 });

  const actionTemplate = useCallback((rowData: OutputProfileDto) => {
    return (
      <div className="flex justify-content-center align-items-center">
        <RemoveOutputProfileDialog outputProfileDto={rowData} />
        {/* <StreamGroupDeleteDialog streamGroup={rowData} /> */}
        {/* <M3UFileRefreshDialog selectedFile={rowData} />
         <M3UFileRemoveDialog selectedFile={rowData} /> */}
        {/* <EPGFileEditDialog selectedFile={rowData} /> */}
      </div>
    );
  }, []);

  const update = useCallback((request: UpdateOutputProfileRequest) => {
    console.log('update', request);

    UpdateOutputProfile(request)
      .then((res) => {})
      .catch((error) => {
        console.log('error', error);
      })
      .finally();
  }, []);

  const nameTemplate = useCallback(
    (rowData: OutputProfileDto) => {
      if (rowData.IsReadOnly === true || rowData.ProfileName.toLowerCase() === 'default') {
        return <div className="text-container pl-1">{rowData.ProfileName}</div>;
      }
      return (
        <StringEditor
          value={rowData.ProfileName}
          onSave={(e) => {
            if (e !== undefined) {
              const ret = { Name: rowData.ProfileName, NewName: e } as UpdateOutputProfileRequest;
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
      nameColumnConfig,
      epgIdColumnConfig,
      groupColumnConfig,
      iDColumnConfig,
      channelNumberColumnConfig,
      groupTitleColumnConfig,
      iconColumnConfig,

      {
        align: 'center',
        bodyTemplate: actionTemplate,
        field: 'action',
        fieldType: 'actions',
        filter: false,
        width: 20
      }
    ],
    [
      nameTemplate,
      nameColumnConfig,
      epgIdColumnConfig,
      groupColumnConfig,
      iDColumnConfig,
      channelNumberColumnConfig,
      groupTitleColumnConfig,
      iconColumnConfig,
      actionTemplate
    ]
  );

  return (
    <SMDataTable
      actionHeaderTemplate={<CreateFileProfileDialog />}
      columns={columns}
      dataSource={data}
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Profiles"
      enableClick
      enableExport={false}
      headerName="M3U/EPG Profiles"
      id={id}
      lazy
      onRowClick={(e: DataTableRowClickEvent) => {}}
      style={{ height: '30vh' }}
    />
  );
};

StreamGroupOutputProfileDataSelector.displayName = 'StreamGroupOutputProfileDataSelector';

export default memo(StreamGroupOutputProfileDataSelector);
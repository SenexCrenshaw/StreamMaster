import StringEditor from '@components/inputs/StringEditor';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { useFilters } from '@lib/redux/hooks/filters';
import { useSortInfo } from '@lib/redux/hooks/sortInfo';

import { Logger } from '@lib/common/logger';
import { UpdateOutputProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import useGetOutputProfiles from '@lib/smAPI/Profiles/useGetOutputProfiles';
import { OutputProfileDto, UpdateOutputProfileRequest } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent } from 'primereact/datatable';
import { memo, useCallback, useMemo } from 'react';
import CreateFileProfileDialog from './CreateFileProfileDialog';
import RemoveOutputProfileDialog from './RemoveOutputProfileDialog';
import { useOutputProfileChannelIdColumnConfig } from './columns/useOutputProfileChannelIdColumnConfig';
import { useOutputProfileChannelNumberColumnConfig } from './columns/useOutputProfileChannelNumberColumnConfig';
import { useOutputProfileGroupTitleColumnConfig } from './columns/useOutputProfileGroupTitleColumnConfig';
import { useOutputProfileTVGGroupColumnConfig } from './columns/useOutputProfileTVGGroupColumnConfig';
import { useOutputProfileTVGIdColumnConfig } from './columns/useOutputProfileTVGIdColumnConfig';
import { useOutputProfileTVGNameColumnConfig } from './columns/useOutputProfileTVGNameColumnConfig';

const StreamGroupOutputProfileDataSelector = () => {
  const id = 'StreamGroupOutputProfileDataSelector';
  const { filters } = useFilters(id);
  const { sortInfo } = useSortInfo(id);
  const { data } = useGetOutputProfiles();

  const tvgNameColumnConfig = useOutputProfileTVGNameColumnConfig({ width: 40 });
  const channelIdColumnConfig = useOutputProfileChannelIdColumnConfig({ width: 40 });
  const tvgGroupColumnConfig = useOutputProfileTVGGroupColumnConfig({ width: 40 });
  const tvgIDColumnConfig = useOutputProfileTVGIdColumnConfig({ width: 40 });
  const groupTitleColumnConfig = useOutputProfileGroupTitleColumnConfig({ width: 40 });
  const channelNumberColumnConfig = useOutputProfileChannelNumberColumnConfig({ width: 40 });

  const filteredValues = useMemo(() => {
    if (!data) {
      return [];
    }

    let ret = [...data];

    if (filters !== undefined && filters['Name'] !== undefined) {
      ret = ret.filter((item: any) => {
        const filterKey = 'Name' as keyof typeof item;
        const itemValue = item[filterKey];
        return typeof itemValue === 'string' && itemValue.toLowerCase().includes(filters['Name'].value.toLowerCase());
      });
    }
    // else {
    //   ret = data.filter((item: any) => {
    //     const filterKey = 'Name' as keyof typeof item;
    //     const itemValue = item[filterKey];

    //     return typeof itemValue === 'string' && itemValue.toLowerCase().includes('hi');
    //   });
    // }

    if (sortInfo !== undefined) {
      ret = ret.sort((a: any, b: any) => {
        const sortField = sortInfo.sortField as keyof typeof a;
        Logger.debug('sortField', sortField, a[sortField], b[sortField]);
        if (a[sortField] < b[sortField]) {
          return -1 * sortInfo.sortOrder;
        }
        if (a[sortField] > b[sortField]) {
          return 1 * sortInfo.sortOrder;
        }
        return 0;
      });
    }

    return ret;
  }, [data, filters, sortInfo]);

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
      if (rowData.IsReadOnly === true || rowData.Name.toLowerCase() === 'default') {
        return <div className="text-container pl-1">{rowData.Name}</div>;
      }
      return (
        <StringEditor
          value={rowData.Name}
          onSave={(e) => {
            if (e !== undefined) {
              const ret = { Name: rowData.Name, NewName: e } as UpdateOutputProfileRequest;
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
        field: 'Name',
        filter: true,
        sortable: true,
        width: 40
      },
      tvgNameColumnConfig,
      channelIdColumnConfig,
      channelNumberColumnConfig,
      tvgIDColumnConfig,
      tvgGroupColumnConfig,
      groupTitleColumnConfig,
      {
        align: 'center',
        bodyTemplate: actionTemplate,
        field: 'action',
        filter: false,
        header: 'Actions',
        width: 20
      }
    ],
    [
      nameTemplate,
      tvgNameColumnConfig,
      channelIdColumnConfig,
      channelNumberColumnConfig,
      tvgIDColumnConfig,
      tvgGroupColumnConfig,
      groupTitleColumnConfig,
      actionTemplate
    ]
  );

  const headerRightTemplate = useMemo(() => {
    return (
      <>
        <CreateFileProfileDialog />
      </>
    );
  }, []);

  return (
    <SMDataTable
      columns={columns}
      dataSource={filteredValues}
      defaultSortField="Name"
      defaultSortOrder={1}
      emptyMessage="No Profiles"
      enableClick
      enableExport={false}
      headerName="M3U/EPG Profiles"
      headerRightTemplate={headerRightTemplate}
      id={id}
      lazy
      onRowClick={(e: DataTableRowClickEvent) => {}}
      style={{ height: '30vh' }}
    />
  );
};

StreamGroupOutputProfileDataSelector.displayName = 'StreamGroupOutputProfileDataSelector';

export default memo(StreamGroupOutputProfileDataSelector);

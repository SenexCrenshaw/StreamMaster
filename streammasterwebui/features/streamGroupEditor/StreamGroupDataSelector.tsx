import DataSelector from '@components/dataSelector/DataSelector';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import StreamGroupAddDialog from '@components/streamGroup/StreamGroupAddDialog';
import StreamGroupDeleteDialog from '@components/streamGroup/StreamGroupDeleteDialog';
import StreamGroupEditDialog from '@components/streamGroup/StreamGroupEditDialog';
import { StreamGroupDto, useStreamGroupsGetPagedStreamGroupsQuery, useStreamGroupsGetStreamGroupQuery } from '@lib/iptvApi';
import { useSelectedStreamGroup } from '@lib/redux/slices/useSelectedStreamGroup';
import { skipToken } from '@reduxjs/toolkit/dist/query/react';
import { memo, useEffect, useMemo, type CSSProperties } from 'react';
export interface StreamGroupDataSelectorProperties {
  readonly id: string;
}

const StreamGroupDataSelector = ({ id }: StreamGroupDataSelectorProperties) => {
  const { selectedStreamGroup, setSelectedStreamGroup } = useSelectedStreamGroup(id);

  const testq = useStreamGroupsGetStreamGroupQuery(selectedStreamGroup?.id ?? skipToken);

  useEffect(() => {
    if (testq.data !== undefined && selectedStreamGroup.id !== undefined) {
      if (testq.data.ffmpegProfileId !== selectedStreamGroup.ffmpegProfileId) {
        setSelectedStreamGroup(testq.data);
      }
      if (testq.data.name !== selectedStreamGroup.name) {
        setSelectedStreamGroup(testq.data);
      }

      if (testq.data.streamCount !== selectedStreamGroup.streamCount) {
        setSelectedStreamGroup(testq.data);
      }
    }
  }, [selectedStreamGroup, setSelectedStreamGroup, testq.data]);

  const StreamGroupColumns = useMemo(
    (): ColumnMeta[] => [
      {
        field: 'name',
        filter: true,
        header: 'Name',
        sortable: true,
        style: {
          minWidth: '10rem'
        } as CSSProperties
      },
      {
        field: 'streamCount',
        header: '#'
      },
      {
        field: 'url',
        fieldType: 'url'
      },
      {
        field: 'epglink',
        fieldType: 'epglink'
      },
      {
        field: 'm3ulink',
        fieldType: 'm3ulink'
      }
    ],
    []
  );

  const sourceAddtionalHeaderTemplate = () => (
    <div className="streamGroupEditor grid w-full flex flex-nowrap justify-content-end align-items-center p-0">
      <div className="flex w-full w-full p-0 align-items-center justify-content-end">
        <div className="flex justify-content-end gap-2 align-items-center mr-2">
          <StreamGroupEditDialog id={id} />
          <StreamGroupAddDialog />
          <StreamGroupDeleteDialog id={id} />
        </div>
      </div>
    </div>
  );

  return (
    <DataSelector
      columns={StreamGroupColumns}
      defaultSortField="name"
      headerName="Stream Groups"
      headerRightTemplate={sourceAddtionalHeaderTemplate()}
      id={`${id}-ds-source`}
      onSelectionChange={(e) => {
        setSelectedStreamGroup(e[0] as StreamGroupDto);
      }}
      queryFilter={useStreamGroupsGetPagedStreamGroupsQuery}
      selectedItemsKey="selectSelectedStreamGroupDtoItems"
      style={{ height: 'calc(100vh - 60px)' }}
    />
  );
};

StreamGroupDataSelector.displayName = 'Stream Group Editor';

export default memo(StreamGroupDataSelector);

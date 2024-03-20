import MinusButton from '@components/buttons/MinusButton';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import { SMChannelDto } from '@lib/apiDefs';
import { GetMessage, arraysContainSameStrings } from '@lib/common/common';
import { ChannelGroupDto } from '@lib/iptvApi';

import { useQueryAdditionalFilters } from '@lib/redux/slices/useQueryAdditionalFilters';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { DeleteSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import useSMChannels from '@lib/smAPI/SMChannels/useSMChannels';
import { ConfirmDialog, confirmDialog } from 'primereact/confirmdialog';

import { Suspense, lazy, memo, useCallback, useEffect, useMemo, useState } from 'react';
const DataSelector2 = lazy(() => import('@components/dataSelector/DataSelector2'));
const StreamCopyLinkDialog = lazy(() => import('@components/smstreams/StreamCopyLinkDialog'));
interface SMChannelDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
  readonly reorderable?: boolean;
}

const SMChannelDataSelector = ({ enableEdit: propsEnableEdit, id, reorderable }: SMChannelDataSelectorProperties) => {
  const dataKey = `${id}-SMChannelDataSelector`;

  const { selectSelectedItems } = useSelectedItems<ChannelGroupDto>('selectSelectedChannelGroupDtoItems');
  const [enableEdit, setEnableEdit] = useState<boolean>(true);

  const channelGroupNames = useMemo(() => selectSelectedItems.map((channelGroup) => channelGroup.name), [selectSelectedItems]);
  const { queryAdditionalFilter, setQueryAdditionalFilter } = useQueryAdditionalFilters(dataKey);

  const { queryFilter } = useQueryFilter(dataKey);
  const { isLoading } = useSMChannels(queryFilter);

  useEffect(() => {
    if (!arraysContainSameStrings(queryAdditionalFilter?.values, channelGroupNames)) {
      setQueryAdditionalFilter({
        field: 'Group',
        matchMode: 'equals',
        values: channelGroupNames
      });
    }
  }, [channelGroupNames, dataKey, queryAdditionalFilter, setQueryAdditionalFilter]);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

  const actionBodyTemplate = useCallback((data: SMChannelDto) => {
    const accept = () => {
      DeleteSMChannel(data.id)
        .then((response) => {})
        .catch((error) => {
          console.error('Remove Channel', error.message);
        });
    };

    const reject = () => {};

    const confirm = () => {
      confirmDialog({
        message: 'Delete "' + data.name + '" ?',
        header: 'Delete Channel',
        icon: 'pi pi-info-circle',
        defaultFocus: 'reject',
        acceptClassName: 'p-button-danger',
        accept,
        reject
      });
    };

    return (
      <div className="flex p-0 justify-content-end align-items-center">
        <Suspense>
          <div className="flex p-0 justify-content-end align-items-center">
            <StreamCopyLinkDialog realUrl={data?.realUrl} />
            <ConfirmDialog />
            <MinusButton iconFilled={false} onClick={confirm} tooltip="Remove Channel" />
          </div>
        </Suspense>
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'channelNumber', width: '4rem' },
      { field: 'logo', fieldType: 'image', width: '4rem' },
      { field: 'name', filter: true, sortable: true },
      { field: 'group', filter: true, sortable: true, width: '5rem' },
      { align: 'right', bodyTemplate: actionBodyTemplate, field: 'isHidden', fieldType: 'actions', header: 'Actions', width: '5rem' }
    ],
    [actionBodyTemplate]
  );

  return (
    <Suspense fallback={<div>Loading...</div>}>
      <DataSelector2
        selectRow
        showExpand
        columns={columns}
        defaultSortField="name"
        defaultSortOrder={1}
        emptyMessage="No Channels"
        headerName={GetMessage('channels').toUpperCase()}
        isLoading={isLoading}
        id={dataKey}
        queryFilter={useSMChannels}
        selectedSMStreamKey="SMChannelDataSelector"
        selectedSMChannelKey="SMChannelDataSelector"
        selectedItemsKey="selectSelectedSMChannelDtoItems"
        style={{ height: 'calc(100vh - 40px)' }}
      />
    </Suspense>
  );
};

export default memo(SMChannelDataSelector);

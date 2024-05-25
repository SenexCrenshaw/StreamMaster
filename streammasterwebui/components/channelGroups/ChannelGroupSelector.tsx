import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import { ProgressSpinner } from 'primereact/progressspinner';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import SMOverlay from '@components/sm/SMOverlay';
import { useSMContext } from '@lib/signalr/SMProvider';
import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import useSelectedAndQ from '@lib/hooks/useSelectedAndQ';
import { useChannelGroupNameColumnConfig } from '@components/columns/ChannelGroups/useChannelGroupNameColumnConfig';
import ChannelGroupVisibleDialog from './ChannelGroupVisibleDialog';
import ChannelGroupAddDialog from './ChannelGroupAddDialog';
import ChannelGroupDeleteDialog from './ChannelGroupDeleteDialog';
import useGetChannelGroupsFromSMChannels from '@lib/smAPI/ChannelGroups/useGetChannelGroupsFromSMChannels';

type ChannelGroupSelectorProperties = {
  readonly darkBackGround?: boolean;
  readonly dataKey: string;
  readonly enableEditMode?: boolean;
  readonly useSelectedItemsFilter?: boolean;
  readonly label?: string;
  readonly value?: string;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
};

const ChannelGroupSelector = ({
  enableEditMode = true,
  dataKey,
  darkBackGround = false,
  label,
  onChange,
  useSelectedItemsFilter,
  value
}: ChannelGroupSelectorProperties) => {
  const { selectedItems, showHidden } = useSelectedAndQ(dataKey);
  const [input, setInput] = useState<string | undefined>(value);
  const { isSystemReady } = useSMContext();
  // const namesQuery = useGetChannelGroups();
  // const sortedData = useSortedData(dataKey, namesQuery.data);

  const namesQuery = useGetChannelGroupsFromSMChannels();

  const { columnConfig: channelGroupNameColumnConfig } = useChannelGroupNameColumnConfig({ enableEdit: true });

  const loading = namesQuery.isLoading || namesQuery.isFetching || !namesQuery.data || isSystemReady !== true;

  useEffect(() => {
    if (value !== undefined) {
      setInput(value);
    }
  }, [value]);

  const dataSource = useMemo(() => {
    if (!namesQuery.data) {
      return [];
    }
    if (showHidden === null) {
      return namesQuery.data;
    }
    return namesQuery.data.filter((x) => (showHidden ? !x.IsHidden : x.IsHidden));
  }, [showHidden, namesQuery]);

  const buttonTemplate = useMemo(() => {
    if (input) {
      if (Array.isArray(input)) {
        if (input.length > 0) {
          const sortedInput = [...input].sort(); // Ensure the input array is sorted
          const suffix = input.length > 2 ? ',...' : '';
          return <div className="text-container">{sortedInput.join(', ') + suffix}</div>;
        }
      }
      return (
        <div className="sm-channelgroup-selector">
          <div className="text-container">{input}</div>
        </div>
      );
    }

    if (selectedItems && selectedItems.length > 0) {
      const names = selectedItems.slice(0, 2).map((x) => x.Name);
      const suffix = selectedItems.length > 2 ? ',...' : '';
      return <div className="text-container">{names.join(', ') + suffix}</div>;
    }

    return <div className="text-container">GROUP</div>;
  }, [input, selectedItems]);

  const actionTemplate = useCallback(
    (data: ChannelGroupDto) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <ChannelGroupVisibleDialog id={dataKey} value={data} />
        <ChannelGroupDeleteDialog id={dataKey} value={data} />
      </div>
    ),
    [dataKey]
  );

  const streamCountTemplate = useCallback(
    (data: ChannelGroupDto) => (
      <div>
        {data.ActiveCount}/{data.TotalCount}
      </div>
    ),
    []
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      channelGroupNameColumnConfig,
      { align: 'left', bodyTemplate: streamCountTemplate, field: 'ActiveCount' },
      { align: 'right', bodyTemplate: actionTemplate, field: 'IsHidden', fieldType: 'actions', header: 'Actions', width: '4rem' }
    ],
    [actionTemplate, channelGroupNameColumnConfig, streamCountTemplate]
  );

  const headerRightTemplate = useMemo(
    () => (
      <>
        <ChannelGroupVisibleDialog id={dataKey} />
        <ChannelGroupDeleteDialog id={dataKey} />
        <ChannelGroupAddDialog />
      </>
    ),
    [dataKey]
  );

  if (loading) {
    return (
      <div className="flex align-content-center justify-content-center text-container">
        <ProgressSpinner className="input-height-with-no-borders" />
      </div>
    );
  }

  if (!enableEditMode) {
    return <div className="flex w-full h-full justify-content-center align-items-center p-0 m-0 text-container">{input ?? 'Dummy'}</div>;
  }

  return (
    <div>
      {label && (
        <div className="stringeditor flex flex-column align-items-start">
          <label className="pl-15">{label.toUpperCase()}</label>
          <div className="pt-small" />
        </div>
      )}
      <SMOverlay
        buttonDarkBackground
        buttonTemplate={buttonTemplate}
        title="GROUPS"
        widthSize="3"
        icon="pi-chevron-down"
        buttonLabel="GROUP"
        header={headerRightTemplate}
      >
        <SMDataTable
          id={dataKey}
          columns={columns}
          noSourceHeader
          dataSource={dataSource}
          selectionMode="multiple"
          showHiddenInSelection
          style={{ height: '40vh' }}
          useSelectedItemsFilter={useSelectedItemsFilter}
          onSelectionChange={(value: any) => {
            if (onChange) {
              onChange(value);
            }
          }}
        />
      </SMOverlay>
    </div>
  );
};

ChannelGroupSelector.displayName = 'ChannelGroupSelector';
export default memo(ChannelGroupSelector);

import { SMOverlay } from '@components/sm/SMOverlay';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { useSMContext } from '@lib/signalr/SMProvider';
import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { ProgressSpinner } from 'primereact/progressspinner';
import React, { useCallback, useEffect, useMemo, useState } from 'react';
import ChannelGroupVisibleDialog from './ChannelGroupVisibleDialog';
import { useChannelGroupNameColumnConfig } from '@components/columns/ChannelGroups/useChannelGroupNameColumnConfig';
import ChannelGroupAddDialog from './ChannelGroupAddDialog';
import ChannelGroupDeleteDialog from './ChannelGroupDeleteDialog';
import useSelectedAndQ from '@lib/hooks/useSelectedAndQ';
import useSortedData from '@components/smDataTable/helpers/useSortedData';

type ChannelGroupSelectorProperties = {
  readonly darkBackGround?: boolean;
  readonly dataKey: string;
  readonly enableEditMode?: boolean;
  readonly disabled?: boolean;
  readonly useSelectedItemsFilter?: boolean;
  readonly editable?: boolean | undefined;
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
  const { selectedItems } = useSelectedAndQ(dataKey);

  const [input, setInput] = useState<string | undefined>(undefined);
  const [originalInput, setOriginalInput] = useState<string | undefined>(undefined);
  const { isSystemReady } = useSMContext();

  const channelGroupQuery = useGetChannelGroups();
  const sortedData = useSortedData(dataKey, channelGroupQuery.data);

  const { columnConfig: channelGroupNameColumnConfig } = useChannelGroupNameColumnConfig({ enableEdit: true });

  const loading = channelGroupQuery.isError || channelGroupQuery.isFetching || channelGroupQuery.isLoading || !channelGroupQuery.data || isSystemReady !== true;

  useEffect(() => {
    if (!originalInput || originalInput !== value) {
      setOriginalInput(value);
      if (value) {
        setInput(value);
        // const found = channelGroupQuery.data?.find((x) => x.Name === value);
        // if (found) {
        //   // setSelectedChannelGroup(found);
        // }
      }
    }
  }, [value, originalInput, channelGroupQuery.data]);

  const dataSource = useMemo(() => {
    return sortedData;
  }, [sortedData]);

  const buttonTemplate = useMemo(() => {
    if (input) {
      console.log('input', input);
      if (Array.isArray(input)) {
        if (input && input.length > 0) {
          const arr = input as ChannelGroupDto[];
          const names = arr.slice(0, 2).map((x) => x.Name);
          const suffix = arr.length > 2 ? ',...' : '';
          return <div className="text-container">{names.join(', ') + suffix}</div>;
        }
      }
      return (
        <div className="sm-channelgroup-selector">
          <div className="text-container ">{input}</div>
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

  const streamCountTemplate = useCallback((data: ChannelGroupDto) => {
    return (
      <div>
        {data.ActiveCount}/{data.TotalCount}
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      channelGroupNameColumnConfig,
      { align: 'left', bodyTemplate: streamCountTemplate, field: 'ActiveCount' },
      { align: 'right', bodyTemplate: actionTemplate, field: 'IsHidden', fieldType: 'actions', header: 'Actions', width: '4rem' }
    ],
    [actionTemplate, channelGroupNameColumnConfig, streamCountTemplate]
  );

  const headerRightTemplate = useMemo(() => {
    return (
      <>
        <ChannelGroupVisibleDialog id={dataKey} />
        <ChannelGroupDeleteDialog id={dataKey} />
        <ChannelGroupAddDialog />
      </>
    );
  }, [dataKey]);

  const getDiv = useMemo(() => {
    let ret = 'input-height-with-no-borders w-11';
    if (darkBackGround === true) {
      ret += ' dark-background sm-input-border-dark';
    }

    return ret;
  }, [darkBackGround]);

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
    <div className={getDiv}>
      {label && (
        <div className="stringeditor flex flex-column align-items-start">
          <label className="pl-15">{label.toUpperCase()}</label>
          <div className="pt-small" />
        </div>
      )}

      <SMOverlay buttonTemplate={buttonTemplate} title="GROUPS" widthSize="3" icon="pi-chevron-down" buttonLabel="GROUP" header={headerRightTemplate}>
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
export default React.memo(ChannelGroupSelector);

import { useChannelGroupNameColumnConfig } from '@components/columns/ChannelGroups/useChannelGroupNameColumnConfig';
import SMDropDown from '@components/sm/SMDropDown';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { QueryHook } from '@lib/apiDefs';
import { useSMContext } from '@lib/context/SMProvider';
import useSelectedAndQ from '@lib/hooks/useSelectedAndQ';
import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { ProgressSpinner } from 'primereact/progressspinner';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import ChannelGroupAddDialog from './ChannelGroupAddDialog';
import ChannelGroupDeleteDialog from './ChannelGroupDeleteDialog';
import ChannelGroupVisibleDialog from './ChannelGroupVisibleDialog';
import { Logger } from '@lib/common/logger';

type BaseChannelGroupSelectorProps = {
  readonly className?: string;
  readonly dataKey: string;
  readonly enableEditMode?: boolean;
  readonly label?: string;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
  readonly useSelectedItemsFilter?: boolean;
  readonly value?: string;
  readonly showTotals?: boolean;
  getNamesQuery: () => ReturnType<QueryHook<ChannelGroupDto[]>>;
};

const BaseChannelGroupSelector = memo(
  ({
    enableEditMode = true,
    dataKey,
    showTotals = true,
    onChange,
    useSelectedItemsFilter,

    value,
    getNamesQuery
  }: BaseChannelGroupSelectorProps) => {
    const { selectedItems } = useSelectedAndQ(dataKey);
    const [input, setInput] = useState<string | undefined>(value);
    const { isSystemReady } = useSMContext();
    const { columnConfig: channelGroupNameColumnConfig } = useChannelGroupNameColumnConfig({ enableEdit: true });
    const namesQuery = getNamesQuery();
    const loading = namesQuery.isLoading || namesQuery.isFetching || !namesQuery.data || isSystemReady !== true;

    useEffect(() => {
      if (value !== undefined) {
        setInput(value);
      }
    }, [value]);

    const buttonTemplate = useMemo(() => {
      if (input) {
        if (Array.isArray(input)) {
          if (input.length > 0) {
            const sortedInput = [...input].sort(); // Ensure the input array is sorted
            const suffix = input.length > 2 ? ',...' : '';
            return <div className="text-container ">{sortedInput.join(', ') + suffix}</div>;
          }
        }
        return <div className="text-container">{input}</div>;
      }

      if (selectedItems && selectedItems.length > 0) {
        const names = selectedItems.slice(0, 2).map((x) => x.Name);
        const suffix = selectedItems.length > 2 ? ',...' : '';
        return <div className="text-container">{names.join(', ') + suffix}</div>;
      }

      return <div className="text-container pl-1">GROUP</div>;
    }, [input, selectedItems]);

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
      (data: ChannelGroupDto) => {
        if (showTotals === true) {
          return (
            <div>
              {data.ActiveCount}/{data.TotalCount}
            </div>
          );
        }
        return <div>{data.ActiveCount}</div>;
      },
      [showTotals]
    );

    const columns = useMemo(
      (): ColumnMeta[] => [
        channelGroupNameColumnConfig,
        { align: 'right', bodyTemplate: streamCountTemplate, field: 'ActiveCount', header: 'Ch Count', width: '5rem' },
        { align: 'right', bodyTemplate: actionTemplate, field: 'IsHidden', fieldType: 'actions', header: '', width: '3rem' }
      ],
      [actionTemplate, channelGroupNameColumnConfig, streamCountTemplate]
    );

    const rowClass = useCallback((data: unknown): string => {
      var channelGroup = data as ChannelGroupDto;
      if (channelGroup.Name.includes('ARNOLD')) {
        Logger.debug('channelGroup', channelGroup.Name, channelGroup.IsHidden);
      }

      if (channelGroup.IsHidden === true) {
        return 'channel-row-selected';
      }

      return '';
    }, []);

    if (loading) {
      return (
        <div className="flex align-content-center justify-content-center text-container">
          <ProgressSpinner className="input-height-with-no-borders" />
        </div>
      );
    }

    if (!enableEditMode) {
      return <div className="flex w-full h-full justify-content-center align-items-center p-0 m-0 text-container">{value ?? 'Dummy'}</div>;
    }

    return (
      <SMDropDown info="" buttonDarkBackground buttonTemplate={buttonTemplate} title="GROUP" contentWidthSize="4" header={headerRightTemplate}>
        <SMDataTable
          columns={columns}
          dataSource={namesQuery.data}
          enablePaginator
          id={dataKey}
          lazy
          noSourceHeader
          onSelectionChange={(value: any) => {
            if (onChange) {
              onChange(value);
            }
          }}
          rowClass={rowClass}
          selectionMode="multiple"
          showClearAll
          showHiddenInSelection
          style={{ height: '50vh' }}
          useSelectedItemsFilter={useSelectedItemsFilter}
        />
      </SMDropDown>
    );
  }
);

export default BaseChannelGroupSelector;

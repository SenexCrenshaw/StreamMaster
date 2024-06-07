import { useChannelGroupNameColumnConfig } from '@components/columns/ChannelGroups/useChannelGroupNameColumnConfig';
import SMOverlay from '@components/sm/SMOverlay';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { QueryHook } from '@lib/apiDefs';
import useSelectedAndQ from '@lib/hooks/useSelectedAndQ';
import { useSMContext } from '@lib/signalr/SMProvider';
import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { DataTableFilterMetaData } from 'primereact/datatable';
import { ProgressSpinner } from 'primereact/progressspinner';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import ChannelGroupAddDialog from './ChannelGroupAddDialog';
import ChannelGroupDeleteDialog from './ChannelGroupDeleteDialog';
import ChannelGroupVisibleDialog from './ChannelGroupVisibleDialog';

// Define a type that includes all shared props plus any additional ones
type BaseChannelGroupSelectorProps = {
  readonly className?: string;
  readonly dataKey: string;
  readonly enableEditMode?: boolean;
  readonly label?: string;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
  readonly useSelectedItemsFilter?: boolean;
  readonly value?: string;
  getNamesQuery: () => ReturnType<QueryHook<ChannelGroupDto[]>>;
};

const BaseChannelGroupSelector = memo(
  ({
    enableEditMode = true,
    dataKey,
    label,
    onChange,
    useSelectedItemsFilter,
    className = 'sm-w-12rem',
    value,
    getNamesQuery
  }: BaseChannelGroupSelectorProps) => {
    const { selectedItems, showHidden, sortInfo, filters } = useSelectedAndQ(dataKey);
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
        return (
          <div className="sm-channelgroup-selector ">
            <div className="text-container">{input}</div>
          </div>
        );
      }

      if (selectedItems && selectedItems.length > 0) {
        const names = selectedItems.slice(0, 2).map((x) => x.Name);
        const suffix = selectedItems.length > 2 ? ',...' : '';
        return (
          <div className="sm-channelgroup-selector">
            <div className="text-container">{names.join(', ') + suffix}</div>
          </div>
        );
      }

      return (
        <div className="sm-channelgroup-selector">
          <div className="text-container pl-1">GROUP</div>
        </div>
      );
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
      (data: ChannelGroupDto) => (
        <div>
          {data.ActiveCount}/{data.TotalCount}
        </div>
      ),
      []
    );

    const dataSource = useMemo(() => {
      if (!namesQuery.data) {
        return [];
      }

      let data = [] as ChannelGroupDto[];

      if (sortInfo?.sortField !== undefined) {
        data = [...namesQuery.data].sort((a, b) => {
          const field = sortInfo.sortField as keyof ChannelGroupDto;
          if (a[field] < b[field]) {
            return sortInfo.sortOrder === 1 ? -1 : 1;
          }
          if (a[field] > b[field]) {
            return sortInfo.sortOrder === 1 ? 1 : -1;
          }
          return 0;
        });

        // Logger.debug('BaseChannelGroupSelector', 'dataSource', {
        //   data: namesQuery.data,
        //   showHidden,
        //   sortedData: sortedData,
        //   sortField: sortInfo.sortField,
        //   sortOrder: sortInfo.sortOrder
        // });
      } else {
        data = namesQuery.data;
      }

      if (showHidden !== null) {
        data = data.filter((x) => (showHidden ? !x.IsHidden : x.IsHidden));
      }

      if (filters.Name !== null) {
        const meta = filters.Name as DataTableFilterMetaData;
        if (meta?.value !== undefined) {
          data = data.filter((x) => x.Name.toLowerCase().includes(meta.value.toLowerCase()));
        }
      }

      return data;
    }, [namesQuery.data, filters, sortInfo, showHidden]);

    const columns = useMemo(
      (): ColumnMeta[] => [
        channelGroupNameColumnConfig,
        { align: 'left', bodyTemplate: streamCountTemplate, field: 'ActiveCount', width: '6rem' },
        { align: 'right', bodyTemplate: actionTemplate, field: 'IsHidden', fieldType: 'actions', header: 'Actions', width: '4rem' }
      ],
      [actionTemplate, channelGroupNameColumnConfig, streamCountTemplate]
    );

    const getDiv = useMemo(() => {
      let div = 'w-full pl-1';
      if (label) {
        div += ' flex-column';
      }

      return div;
    }, [label]);

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
      <div className={className}>
        {label && (
          <div className="flex flex-column align-items-start">
            <label className="pl-15">{label.toUpperCase()}</label>
            <div className="pt-small" />
          </div>
        )}
        <div className={getDiv}>
          <SMOverlay
            buttonDarkBackground
            buttonTemplate={buttonTemplate}
            title="GROUPS"
            contentWidthSize="3"
            icon="pi-chevron-down"
            buttonLabel="GROUP"
            header={headerRightTemplate}
            isLoading={loading}
          >
            <SMDataTable
              columns={columns}
              dataSource={dataSource}
              id={dataKey}
              lazy
              noSourceHeader
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
      </div>
    );
  }
);

export default BaseChannelGroupSelector;

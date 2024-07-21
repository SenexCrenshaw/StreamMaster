import { useChannelGroupNameColumnConfig } from '@components/columns/ChannelGroups/useChannelGroupNameColumnConfig';
import SMDropDown from '@components/sm/SMDropDown';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { QueryHook } from '@lib/apiDefs';
import { useSMContext } from '@lib/context/SMProvider';
import useSelectedAndQ from '@lib/hooks/useSelectedAndQ';
import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';
import { BlockUI } from 'primereact/blockui';
import { ProgressSpinner } from 'primereact/progressspinner';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import ChannelGroupAddDialog from './ChannelGroupAddDialog';
import ChannelGroupDeleteDialog from './ChannelGroupDeleteDialog';
import ChannelGroupVisibleDialog from './ChannelGroupVisibleDialog';

type BaseChannelGroupSelectorProps = {
  readonly className?: string;
  readonly dataKey: string;
  readonly enableEditMode?: boolean;
  readonly label?: string;
  readonly onChange?: (value: ChannelGroupDto[]) => void;
  readonly useSelectedItemsFilter?: boolean;
  readonly value?: string;
  readonly autoPlacement?: boolean;
  getNamesQuery: () => ReturnType<QueryHook<ChannelGroupDto[]>>;
};

const BaseChannelGroupSelector = memo(
  ({
    enableEditMode = true,
    dataKey,
    label,
    autoPlacement = false,
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
        { align: 'left', bodyTemplate: streamCountTemplate, field: 'ActiveCount', width: '5rem' },
        { align: 'right', bodyTemplate: actionTemplate, field: 'IsHidden', fieldType: 'actions', header: '', width: '3rem' }
      ],
      [actionTemplate, channelGroupNameColumnConfig, streamCountTemplate]
    );

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
      <>
        <BlockUI blocked={loading}>
          <SMDropDown info="" buttonDarkBackground buttonTemplate={buttonTemplate} title="GROUP" contentWidthSize="3" header={headerRightTemplate}>
            <SMDataTable
              columns={columns}
              dataSource={namesQuery.data}
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
          </SMDropDown>
        </BlockUI>
      </>
    );

    //   <div className={className}>
    //     {label && (
    //       <div className="flex flex-column align-items-start">
    //         <label className="pl-15">{label.toUpperCase()}</label>
    //         <div className="pt-small" />
    //       </div>
    //     )}
    //     <div className={getDiv}>
    //       <SMOverlay
    //         buttonDarkBackground
    //         buttonLabel="GROUP"
    //         buttonTemplate={buttonTemplate}
    //         contentWidthSize="3"
    //         autoPlacement={autoPlacement}
    //         header={headerRightTemplate}
    //         icon="pi-chevron-down"
    //         isLoading={loading}
    //         title="GROUPS"
    //       >
    //         <SMDataTable
    //           columns={columns}
    //           dataSource={dataSource}
    //           id={dataKey}
    //           lazy
    //           noSourceHeader
    //           selectionMode="multiple"
    //           showHiddenInSelection
    //           style={{ height: '40vh' }}
    //           useSelectedItemsFilter={useSelectedItemsFilter}
    //           onSelectionChange={(value: any) => {
    //             if (onChange) {
    //               onChange(value);
    //             }
    //           }}
    //         />
    //       </SMOverlay>
    //     </div>
    //   </div>
    // );
  }
);

export default BaseChannelGroupSelector;

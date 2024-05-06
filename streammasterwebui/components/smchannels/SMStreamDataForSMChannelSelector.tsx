import AddButton from '@components/buttons/AddButton';
import MinusButton from '@components/buttons/MinusButton';

import getRecord from '@components/smDataTable/helpers/getRecord';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { GetMessage } from '@lib/common/common';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';

import { TriSelectShowHidden } from '@components/selectors/TriSelectShowHidden';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import useGetPagedSMStreams from '@lib/smAPI/SMStreams/useGetPagedSMStreams';
import { SMStreamDto } from '@lib/smAPI/smapiTypes';
import { Suspense, lazy, memo, useCallback, useEffect, useMemo, useState } from 'react';

const SMDataTable = lazy(() => import('@components/smDataTable/SMDataTable'));

interface SMStreamDataForSMChannelSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
  readonly height?: string;
  readonly showSelections?: boolean;
  readonly name: string | undefined;
}

const SMStreamDataForSMChannelSelector = ({ enableEdit: propsEnableEdit, height, id, name, showSelections }: SMStreamDataForSMChannelSelectorProperties) => {
  const dataKey = `${id}-SMStreamDataForSMChannelSelector`;
  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems<SMStreamDto>(dataKey);

  const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { queryFilter } = useQueryFilter(dataKey);

  const { isLoading } = useGetPagedSMStreams(queryFilter);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'Name', filter: true, sortable: true, width: '8rem' },
      { field: 'Group', filter: true, sortable: true, maxWidth: '4rem' },
      { field: 'M3UFileName', filter: true, header: 'M3U', maxWidth: '4rem', sortable: true }
    ],
    []
  );

  const addOrRemoveTemplate = useCallback(
    (data: any) => {
      const found = selectSelectedItems?.some((item) => item.Id === data.Id) ?? false;

      let toolTip = '';

      if (found) {
        if (name !== undefined) {
          toolTip = 'Remove Stream From "' + name + '"';
        } else {
          toolTip = 'Remove Stream';
        }
        return (
          <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
            <MinusButton
              iconFilled={false}
              onClick={() => {
                if (!data.Id) {
                  return;
                }
                const newData = selectSelectedItems?.filter((item) => item.Id !== data.Id);
                setSelectSelectedItems(newData);
              }}
              tooltip={toolTip}
            />
          </div>
        );
      }
      if (name !== undefined) {
        toolTip = 'Add Stream To "' + name + '"';
      } else {
        toolTip = 'Add Stream';
      }
      toolTip = 'Add Stream To "' + name + '"';
      return (
        <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
          <AddButton
            iconFilled={false}
            onClick={() => {
              setSelectSelectedItems([...(selectSelectedItems ?? []), data]);
            }}
            tooltip={toolTip}
          />

          {/* {showSelection && <Checkbox checked={isSelected} className="pl-1" onChange={() => addSelection(data)} />} */}
        </div>
      );
    },
    [name, selectSelectedItems, setSelectSelectedItems]
  );

  function addOrRemoveHeaderTemplate() {
    return <TriSelectShowHidden dataKey={dataKey} />;
    // const isSelected = false;

    // if (!isSelected) {
    //   return (
    //     <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
    //       {/* <AddButton iconFilled={false} onClick={() => console.log('AddButton')} tooltip="Add All Channels" /> */}
    //       {/* {showSelection && <Checkbox checked={state.selectAll} className="pl-1" onChange={() => toggleAllSelection()} />} */}
    //     </div>
    //   );
    // }

    // return (
    //   <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
    //     <AddButton iconFilled={false} onClick={() => console.log('AddButton')} />
    //     {/* {showSelection && <Checkbox checked={state.selectAll} className="pl-1" onChange={() => toggleAllSelection()} />} */}
    //   </div>
    // );
  }

  // const rightHeaderTemplate = useMemo(
  //   () => (
  //     <div className="flex flex-row justify-content-end align-items-center w-full gap-2 pr-2">
  //       <div>
  //         <M3UFilesButton />
  //       </div>
  //       <BaseButton className="button-red" icon="pi pi-times" rounded onClick={() => {}} />
  //       <BaseButton className="button-yellow" icon="pi-plus" rounded onClick={() => {}} />
  //       <BaseButton className="button-orange" icon="pi pi-bars" rounded onClick={() => {}} />

  //       <StreamMultiVisibleDialog iconFilled selectedItemsKey="selectSelectedSMStreamDtoItems" id={dataKey} skipOverLayer />
  //     </div>
  //   ),
  //   [dataKey]
  // );

  const rowClass = useCallback(
    (data: unknown): string => {
      const isHidden = getRecord(data, 'IsHidden');

      if (isHidden === true) {
        return 'bg-red-900';
      }

      if (data && selectSelectedItems && selectSelectedItems !== undefined && Array.isArray(selectSelectedItems)) {
        const id = getRecord(data, 'Id');
        if (selectSelectedItems.some((stream) => stream.Id === id)) {
          return 'bg-blue-900';
        }
      }

      return '';
    },
    [selectSelectedItems]
  );

  return (
    <Suspense>
      <SMDataTable
        columns={columns}
        defaultSortField="Name"
        defaultSortOrder={1}
        addOrRemoveTemplate={addOrRemoveTemplate}
        addOrRemoveHeaderTemplate={addOrRemoveHeaderTemplate}
        enablePaginator
        emptyMessage="No Streams"
        headerName={GetMessage('m3ustreams').toUpperCase()}
        isLoading={isLoading}
        id={dataKey}
        rowClass={rowClass}
        queryFilter={useGetPagedSMStreams}
        style={{ height: height ?? 'calc(100vh - 100px)' }}
      />
    </Suspense>
  );
};

export default memo(SMStreamDataForSMChannelSelector);

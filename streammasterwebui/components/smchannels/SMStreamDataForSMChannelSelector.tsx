import AddButton from '@components/buttons/AddButton';
import MinusButton from '@components/buttons/MinusButton';

import getRecord from '@components/smDataTable/helpers/getRecord';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import { GetMessage } from '@lib/common/common';
import { useQueryFilter } from '@lib/redux/slices/useQueryFilter';

import { TriSelectShowHidden } from '@components/selectors/TriSelectShowHidden';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { AddSMStreamToSMChannel, RemoveSMStreamFromSMChannel } from '@lib/smAPI/SMChannelStreamLinks/SMChannelStreamLinksCommands';
import useGetPagedSMStreams from '@lib/smAPI/SMStreams/useGetPagedSMStreams';
import { RemoveSMStreamFromSMChannelRequest, SMChannelDto, SMStreamDto } from '@lib/smAPI/smapiTypes';
import { Suspense, lazy, memo, useCallback, useEffect, useMemo, useState } from 'react';

const SMDataTable = lazy(() => import('@components/smDataTable/SMDataTable'));

interface SMStreamDataForSMChannelSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
  readonly height?: string;

  readonly name: string | undefined;
  readonly smChannel?: SMChannelDto;
}

const SMStreamDataForSMChannelSelector = ({ enableEdit: propsEnableEdit, height, id, name, smChannel }: SMStreamDataForSMChannelSelectorProperties) => {
  const dataKey = `${id}-SMStreamDataForSMChannelSelector`;
  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems<SMStreamDto>(`${id}-SMStreamDataForSMChannelSelector`);

  const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { queryFilter } = useQueryFilter(dataKey);
  console.log('queryFilter', queryFilter);

  const { isLoading } = useGetPagedSMStreams(queryFilter);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'Name', filter: true, sortable: true, width: '8rem' },
      { field: 'Group', filter: true, maxWidth: '4rem', sortable: true },
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

                if (smChannel) {
                  const request = {} as RemoveSMStreamFromSMChannelRequest;
                  request.SMChannelId = smChannel.Id;
                  request.SMStreamId = data.Id;
                  RemoveSMStreamFromSMChannel(request)
                    .then((response) => {
                      console.log('Remove Stream', response);
                    })
                    .catch((error) => {
                      console.error('Remove Stream', error.message);
                    });
                } else {
                  const newData = selectSelectedItems?.filter((item) => item.Id !== data.Id);
                  setSelectSelectedItems(newData);
                }
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
              if (smChannel) {
                AddSMStreamToSMChannel({ SMChannelId: smChannel?.Id ?? 0, SMStreamId: data.Id })
                  .then((response) => {})
                  .catch((error) => {
                    console.error(error.message);
                    throw error;
                  });
              } else {
                setSelectSelectedItems([...(selectSelectedItems ?? []), data]);
              }
            }}
            tooltip={toolTip}
          />
        </div>
      );
    },
    [name, selectSelectedItems, setSelectSelectedItems, smChannel]
  );

  function addOrRemoveHeaderTemplate() {
    return <TriSelectShowHidden dataKey={dataKey} />;
  }

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

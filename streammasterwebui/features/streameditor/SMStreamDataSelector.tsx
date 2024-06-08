import { useSMStreamGroupColumnConfig } from '@components/columns/SMStreams/useSMStreamGroupColumnConfig';
import { useSMStreamM3UColumnConfig } from '@components/columns/SMStreams/useSMStreamM3UColumnConfig';
import M3UFilesButton from '@components/m3u/M3UFilesButton';
import SMButton from '@components/sm/SMButton';
import { SMTriSelectShowHidden } from '@components/sm/SMTriSelectShowHidden';
import getRecord from '@components/smDataTable/helpers/getRecord';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import CreateSMChannelsDialog from '@components/smchannels/CreateSMChannelsDialog';
import StreamCopyLinkDialog from '@components/smstreams/StreamCopyLinkDialog';
import StreamMultiVisibleDialog from '@components/smstreams/StreamMultiVisibleDialog';
import StreamVisibleDialog from '@components/smstreams/StreamVisibleDialog';
import { GetMessage } from '@lib/common/intl';
import { useIsTrue } from '@lib/redux/hooks/isTrue';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectedSMStreams } from '@lib/redux/hooks/selectedSMStreams';

import CreateSMStreamDialog from '@components/smstreams/CreateSMStreamDialog';
import EditSMStreamDialog from '@components/smstreams/EditSMStreamDialog';
import { Logger } from '@lib/common/logger';
import { AddSMStreamToSMChannel, RemoveSMStreamFromSMChannel } from '@lib/smAPI/SMChannelStreamLinks/SMChannelStreamLinksCommands';
import { CreateSMChannelFromStream } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import useGetPagedSMStreams from '@lib/smAPI/SMStreams/useGetPagedSMStreams';
import { CreateSMChannelFromStreamRequest, RemoveSMStreamFromSMChannelRequest, SMChannelDto, SMStreamDto } from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent, DataTableRowEvent, DataTableValue } from 'primereact/datatable';
import { Suspense, lazy, memo, useCallback, useEffect, useMemo, useState } from 'react';
import SimpleButton from '../../components/buttons/SimpleButton';
import useSelectedSMItems from './useSelectedSMItems';

const SMDataTable = lazy(() => import('@components/smDataTable/SMDataTable'));
interface SMStreamDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
  readonly height?: string;
  readonly showSelections?: boolean;
  readonly simple?: boolean;
}

const SMStreamDataSelector = ({ enableEdit: propsEnableEdit, height, id, simple = false, showSelections }: SMStreamDataSelectorProperties) => {
  const dataKey = `${id}-SMStreamDataSelector`;
  const { isTrue: smTableIsSimple } = useIsTrue(dataKey);

  const { selectedSMChannel, setSelectedSMChannel } = useSelectedSMItems();

  const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { setSelectedSMStreams } = useSelectedSMStreams(dataKey);
  const groupColumnConfig = useSMStreamGroupColumnConfig({ dataKey });
  const smStreamM3UColumnConfig = useSMStreamM3UColumnConfig({ dataKey });
  const { queryFilter } = useQueryFilter(dataKey);
  const { isLoading } = useGetPagedSMStreams(queryFilter);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

  const actionTemplate = useCallback((data: SMStreamDto) => {
    Logger.debug('SMStreamDataSelector actionTemplate', data);
    return (
      <div className="flex p-0 justify-content-end align-items-center">
        <StreamCopyLinkDialog realUrl={data.RealUrl} />
        <StreamVisibleDialog iconFilled={false} value={data} />
        <EditSMStreamDialog smStreamDto={data} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'Name', filter: true, sortable: true, width: '16rem' },
      groupColumnConfig,
      smStreamM3UColumnConfig,
      { align: 'right', bodyTemplate: actionTemplate, field: 'IsHidden', fieldType: 'actions', header: 'Actions', width: '4rem' }
    ],
    [actionTemplate, groupColumnConfig, smStreamM3UColumnConfig]
  );

  const simpleColumns = useMemo((): ColumnMeta[] => [{ field: 'Name', filter: true, sortable: true, width: '10rem' }], []);

  const addOrRemoveTemplate = useCallback(
    (data: any) => {
      const found = selectedSMChannel?.SMStreams?.some((item) => item.Id === data.Id) ?? false;

      let toolTip = 'Add Channel';
      if (selectedSMChannel !== undefined) {
        toolTip = 'Remove Stream From "' + selectedSMChannel.Name + '"';
        if (found)
          return (
            <div className="flex align-content-center justify-content-center">
              <SMButton
                icon="pi-minus"
                className="border-noround borderread icon-red"
                iconFilled={false}
                onClick={() => {
                  if (!data.Id || selectedSMChannel === undefined) {
                    return;
                  }
                  const request: RemoveSMStreamFromSMChannelRequest = { SMChannelId: selectedSMChannel.Id, SMStreamId: data.Id };
                  RemoveSMStreamFromSMChannel(request)
                    .then((response) => {
                      console.log('Remove Stream', response);
                    })
                    .catch((error) => {
                      console.error('Remove Stream', error.message);
                    });
                }}
                tooltip={toolTip}
              />
            </div>
          );

        toolTip = 'Add Stream To "' + selectedSMChannel.Name + '"';
        return (
          <div className="flex align-content-center justify-content-center">
            <SMButton
              icon="pi-plus"
              className="border-noround borderread icon-green"
              iconFilled={false}
              onClick={() => {
                AddSMStreamToSMChannel({ SMChannelId: selectedSMChannel?.Id ?? 0, SMStreamId: data.Id })
                  .then((response) => {})
                  .catch((error) => {
                    console.error(error.message);
                    throw error;
                  });
              }}
              tooltip={toolTip}
            />
          </div>
        );
      }

      return (
        <div className="flex align-content-center justify-content-center">
          <SMButton
            icon="pi-plus"
            iconFilled={false}
            className="border-noround borderread icon-green"
            onClick={() => {
              CreateSMChannelFromStream({ StreamId: data.Id } as CreateSMChannelFromStreamRequest)
                .then((response) => {})
                .catch((error) => {});
            }}
            tooltip={toolTip}
          />
        </div>
      );
    },
    [selectedSMChannel]
  );

  function addOrRemoveHeaderTemplate() {
    return (
      <div className="flex align-content-center justify-content-center">
        <SMTriSelectShowHidden dataKey={dataKey} />
      </div>
    );
  }

  const rightHeaderTemplate = useMemo(() => {
    if (smTableIsSimple) {
      return (
        <div className="flex justify-content-end align-items-center w-full gap-1 pr-1">
          {!smTableIsSimple && <M3UFilesButton />}
          <SimpleButton dataKey={dataKey} />
          <SMButton className="icon-orange" iconFilled icon="pi pi-bars" rounded onClick={() => {}} />
        </div>
      );
    }
    return (
      <div className="flex justify-content-end align-items-center w-full gap-1 pr-1">
        <M3UFilesButton />
        <SimpleButton dataKey={dataKey} />
        <StreamMultiVisibleDialog selectedItemsKey="selectSelectedSMStreamDtoItems" id={dataKey} />
        <CreateSMChannelsDialog selectedItemsKey="selectSelectedSMStreamDtoItems" id={dataKey} />
        <CreateSMStreamDialog />
      </div>
    );
  }, [dataKey, smTableIsSimple]);

  const setSelectedSMEntity = useCallback(
    (data: DataTableValue, toggle?: boolean) => {
      if (toggle === true && selectedSMChannel !== undefined && data !== undefined && data.id === selectedSMChannel.Id) {
        setSelectedSMChannel(undefined);
      } else {
        setSelectedSMChannel(data as SMChannelDto);
      }
    },
    [selectedSMChannel, setSelectedSMChannel]
  );

  const rowClass = useCallback(
    (data: unknown): string => {
      const isHidden = getRecord(data, 'IsHidden');

      if (isHidden === true) {
        return 'bg-red-900';
      }

      if (data && selectedSMChannel?.SMStreams !== undefined && selectedSMChannel.SMStreams.length > 0) {
        const id = getRecord(data, 'Id');
        if (selectedSMChannel.SMStreams.some((stream) => stream.Id === id)) {
          return 'channel-row-selected';
        }
      }

      return '';
    },
    [selectedSMChannel]
  );

  return (
    <Suspense>
      <SMDataTable
        columns={smTableIsSimple ? simpleColumns : columns}
        defaultSortField="Name"
        defaultSortOrder={1}
        addOrRemoveTemplate={addOrRemoveTemplate}
        addOrRemoveHeaderTemplate={addOrRemoveHeaderTemplate}
        enablePaginator
        emptyMessage="No Streams"
        headerName={GetMessage('m3ustreams').toUpperCase()}
        headerRightTemplate={simple === true ? undefined : rightHeaderTemplate}
        isLoading={isLoading}
        id={dataKey}
        onSelectionChange={(value, selectAll) => {
          if (selectAll !== true) {
            setSelectedSMStreams(value as SMStreamDto[]);
          }
        }}
        onClick={(e: any) => {
          if (e.target.className && e.target.className === 'p-datatable-wrapper') {
            setSelectedSMChannel(undefined);
          }
        }}
        onRowExpand={(e: DataTableRowEvent) => {
          setSelectedSMEntity(e.data);
        }}
        onRowClick={(e: DataTableRowClickEvent) => {
          setSelectedSMEntity(e.data, true);
        }}
        rowClass={rowClass}
        queryFilter={useGetPagedSMStreams}
        selectedItemsKey="selectSelectedSMStreamDtoItems"
        selectionMode="multiple"
        style={{ height: height ?? 'calc(100vh - 100px)' }}
      />
    </Suspense>
  );
};

export default memo(SMStreamDataSelector);

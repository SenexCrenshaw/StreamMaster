import M3UFilesButton from '@components/m3u/M3UFilesButton';
import SMButton from '@components/sm/SMButton';
import { SMTriSelectShowHidden } from '@components/sm/SMTriSelectShowHidden';
import getRecord from '@components/smDataTable/helpers/getRecord';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import StreamVisibleDialog from '@components/smstreams/StreamVisibleDialog';
import { useSMStreamM3UColumnConfig } from '@components/smstreams/columns/useSMStreamM3UColumnConfig';
import { GetMessage } from '@lib/common/intl';
import { useIsTrue } from '@lib/redux/hooks/isTrue';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectedSMStreams } from '@lib/redux/hooks/selectedSMStreams';

import SMDataTable from '@components/smDataTable/SMDataTable';
import DeleteSMStreamDialog from '@components/smstreams/DeleteSMStreamDialog';
import EditSMStreamDialog from '@components/smstreams/EditSMStreamDialog';
import { useSMStreamChannelGroupColumnConfig } from '@components/smstreams/columns/useSMStreamChannelGroupColumnConfig';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { AddSMStreamToSMChannel, RemoveSMStreamFromSMChannel } from '@lib/smAPI/SMChannelStreamLinks/SMChannelStreamLinksCommands';

import { LinkButton } from '@components/buttons/LinkButton';
import { useSMStreamMembershipColumnConfig } from '@components/smstreams/columns/useSMStreamMembershipColumnConfig';
import { CreateSMChannelsFromStreams } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import useGetPagedSMStreams from '@lib/smAPI/SMStreams/useGetPagedSMStreams';
import {
  AddSMStreamToSMChannelRequest,
  CreateSMChannelsFromStreamsRequest,
  RemoveSMStreamFromSMChannelRequest,
  SMChannelDto,
  SMStreamDto
} from '@lib/smAPI/smapiTypes';
import { DataTableRowClickEvent, DataTableRowEvent, DataTableValue } from 'primereact/datatable';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';
import SimpleButton from '../../components/buttons/SimpleButton';
import SMStreamMenu from './SMStreamMenu';
import useSelectedSMItems from './useSelectedSMItems';

interface SMStreamDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
  readonly height?: string;
  readonly showSelections?: boolean;
  readonly simple?: boolean;
}

const SMStreamDataSelector = ({ enableEdit: propsEnableEdit, height, id, simple = false, showSelections }: SMStreamDataSelectorProperties) => {
  const dataKey = `${id}-SMStreamDataSelector`;
  const { isTrue: smTableIsSimple } = useIsTrue('isSimple');
  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const { selectedSMChannel, setSelectedSMChannel } = useSelectedSMItems();

  const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { setSelectedSMStreams } = useSelectedSMStreams(dataKey);
  const groupColumnConfig = useSMStreamChannelGroupColumnConfig({ dataKey });
  const smStreamM3UColumnConfig = useSMStreamM3UColumnConfig({ dataKey });
  const smStreamMembershipColumnConfig = useSMStreamMembershipColumnConfig({ dataKey });
  const { queryFilter } = useQueryFilter(dataKey);
  const { isLoading } = useGetPagedSMStreams(queryFilter);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

  const actionTemplate = useCallback((data: SMStreamDto) => {
    return (
      <div className="flex justify-content-end align-items-center" style={{ paddingRight: '0.1rem' }}>
        <LinkButton bolt link={data.Url} toolTip="Original Source UR" title="Original Source Link" />
        {/* <StreamCopyLinkDialog realUrl={data.RealUrl} /> */}
        <StreamVisibleDialog iconFilled={false} value={data} />
        <EditSMStreamDialog smStreamDto={data} />
        <DeleteSMStreamDialog smStream={data} />
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'Name', filter: true, sortable: true, width: 200 },
      // { align: 'right', field: 'ChannelNumber', header: '#', sortable: true, width: 40 },
      smStreamMembershipColumnConfig,
      groupColumnConfig,
      smStreamM3UColumnConfig,
      { align: 'right', field: 'M3UFileId', fieldType: 'filterOnly' },
      { align: 'right', bodyTemplate: actionTemplate, field: 'IsHidden', fieldType: 'actions', header: 'Actions', width: 108 }
    ],
    [actionTemplate, groupColumnConfig, smStreamM3UColumnConfig, smStreamMembershipColumnConfig]
  );

  const simpleColumns = useMemo((): ColumnMeta[] => [{ field: 'Name', filter: true, sortable: true, width: '10rem' }], []);

  const addOrRemoveTemplate = useCallback(
    (data: any) => {
      const smStream = data as unknown as SMStreamDto;

      if (smStream.SMStreamType.toString() === 'CustomPlayList') {
        return <div className="flex align-content-center justify-content-center" />;
      }
      const found = selectedSMChannel?.SMStreamDtos?.some((item) => item.Id === smStream.Id) ?? false;

      let toolTip = 'Add Channel';
      if (selectedSMChannel !== undefined) {
        toolTip = 'Remove Stream From "' + selectedSMChannel.Name + '"';
        if (found)
          return (
            <div className="flex align-content-center justify-content-center">
              <SMButton
                icon="pi-minus"
                buttonClassName="border-noround borderread icon-red"
                iconFilled={false}
                onClick={() => {
                  if (!smStream.Id || selectedSMChannel === undefined) {
                    return;
                  }
                  const request: RemoveSMStreamFromSMChannelRequest = { SMChannelId: selectedSMChannel.Id, SMStreamId: smStream.Id };
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
              buttonClassName="border-noround borderread icon-green"
              iconFilled={false}
              onClick={() => {
                const request = { SMChannelId: selectedSMChannel?.Id ?? 0, SMStreamId: smStream.Id } as AddSMStreamToSMChannelRequest;

                AddSMStreamToSMChannel(request)
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
            buttonClassName="border-noround borderread icon-green"
            onClick={() => {
              const request = { StreamIds: [smStream.Id] } as CreateSMChannelsFromStreamsRequest;
              if (selectedStreamGroup?.Id) {
                request.StreamGroupId = selectedStreamGroup.Id;
              }
              CreateSMChannelsFromStreams(request)
                .then((response) => {})
                .catch((error) => {});
            }}
            tooltip={toolTip}
          />
        </div>
      );
    },
    [selectedSMChannel, selectedStreamGroup]
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
          <M3UFilesButton />
          {!smTableIsSimple && <M3UFilesButton />}
          <SimpleButton dataKey="isSimple" />
          <SMStreamMenu />
        </div>
      );
    }
    return (
      <div className="flex justify-content-end align-items-center w-full gap-1 pr-1">
        <M3UFilesButton />
        <SimpleButton dataKey="isSimple" />
        {/* <StreamMultiVisibleDialog selectedItemsKey="selectSelectedSMStreamDtoItems" id={dataKey} />
        <CreateSMChannelsDialog selectedItemsKey="selectSelectedSMStreamDtoItems" id={dataKey} /> */}
        {/* <CreateSMStreamDialog /> */}
        <SMStreamMenu />
      </div>
    );
  }, [smTableIsSimple]);

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
      if (data === null || data === undefined) {
        // Logger.debug('SMStreamDataSelector', 'rowClass', 'data is null or undefined');
        return '';
      }
      const isHidden = getRecord({ data, fieldName: 'IsHidden' });

      if (isHidden === true) {
        return 'bg-red-900';
      }

      if (data && selectedSMChannel?.SMStreamDtos !== undefined && selectedSMChannel.SMStreamDtos.length > 0) {
        const id = getRecord({ data, fieldName: 'Id' });
        if (id === undefined) {
          // Logger.debug('SMStreamDataSelector', 'rowClass', 'Id is undefined');
          return '';
        }

        selectedSMChannel.SMStreamDtos.forEach((element) => {
          if (element?.Id === undefined) {
            // Logger.debug('SMStreamDataSelector', 'element', element);
            return true;
          }
        });

        if (selectedSMChannel.SMStreamDtos.some((stream) => stream.Id === id)) {
          return 'channel-row-selected';
        }
      }

      return '';
    },
    [selectedSMChannel]
  );

  return (
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
      queryFilter={useGetPagedSMStreams}
      rowClass={rowClass}
      selectedItemsKey="selectSelectedSMStreamDtoItems"
      selectionMode="multiple"
      showSelectAll
      style={{ height: height ?? 'calc(100vh - 100px)' }}
    />
  );
};

export default memo(SMStreamDataSelector);

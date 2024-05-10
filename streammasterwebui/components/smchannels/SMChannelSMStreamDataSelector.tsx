import M3UFilesButton from '@components/m3u/M3UFilesButton';
import SMDataTable from '@components/smDataTable/SMDataTable';
import { ColumnMeta } from '@components/smDataTable/types/ColumnMeta';
import StreamCopyLinkDialog from '@components/smstreams/StreamCopyLinkDialog';
import StreamVisibleDialog from '@components/smstreams/StreamVisibleDialog';
import { GetMessage } from '@lib/common/common';

import { useSMStreamGroupColumnConfig } from '@components/columns/SMStreams/useSMChannelGroupColumnConfig';
import { useSMStreamM3UColumnConfig } from '@components/columns/SMStreams/useSMStreamM3UColumnConfig';
import SMButton from '@components/sm/SMButton';
import StreamMultiVisibleDialog from '@components/smstreams/StreamMultiVisibleDialog';
import { useSelectedSMStreams } from '@lib/redux/hooks/selectedSMStreams';
import useGetSMChannelStreams from '@lib/smAPI/SMChannelStreamLinks/useGetSMChannelStreams';
import { GetSMChannelStreamsRequest, SMStreamDto } from '@lib/smAPI/smapiTypes';
import { memo, useCallback, useEffect, useMemo, useState } from 'react';

interface SSMChannelSMStreamDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id?: string;
  readonly showSelections?: boolean;
  readonly smChannelId?: number;
}

const SMChannelSMStreamDataSelector = ({ enableEdit: propsEnableEdit, id, smChannelId, showSelections }: SSMChannelSMStreamDataSelectorProperties) => {
  const dataKey = `${id}-SMChannelSMStreamDataSelector`;

  const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { setSelectedSMStreams } = useSelectedSMStreams(dataKey);
  const groupColumnConfig = useSMStreamGroupColumnConfig();
  const smStreamM3UColumnConfig = useSMStreamM3UColumnConfig();

  const { data, isLoading } = useGetSMChannelStreams({ SMChannelId: smChannelId } as GetSMChannelStreamsRequest);

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

  const actionBodyTemplate = useCallback(
    (data: SMStreamDto) => (
      <div className="flex p-0 justify-content-end align-items-center">
        <StreamCopyLinkDialog realUrl={data.RealUrl} />
        <StreamVisibleDialog iconFilled={false} value={data} />
      </div>
    ),
    []
  );

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'Name', filter: true, sortable: true, width: '18rem' },
      groupColumnConfig,
      smStreamM3UColumnConfig,
      { align: 'right', bodyTemplate: actionBodyTemplate, field: 'IsHidden', fieldType: 'actions', header: 'Actions', width: '4rem' }
    ],
    [actionBodyTemplate, groupColumnConfig, smStreamM3UColumnConfig]
  );

  const rightHeaderTemplate = useMemo(
    () => (
      <div className="flex flex-row justify-content-end align-items-center w-full gap-1 pr-2">
        <M3UFilesButton />
        <SMButton className="button-red" icon="pi pi-times" rounded onClick={() => {}} />
        <SMButton className="button-yellow" icon="pi-plus" rounded onClick={() => {}} />
        <SMButton className="button-orange" icon="pi pi-bars" rounded onClick={() => {}} />
        <StreamMultiVisibleDialog iconFilled selectedItemsKey="selectSelectedSMStreamDtoItems" id={dataKey} skipOverLayer />
      </div>
    ),
    [dataKey]
  );

  return (
    <SMDataTable
      columns={columns}
      defaultSortField="Name"
      defaultSortOrder={1}
      dataSource={data}
      enablePaginator
      emptyMessage="No Streams"
      headerName={GetMessage('m3ustreams').toUpperCase()}
      headerRightTemplate={rightHeaderTemplate}
      isLoading={isLoading}
      id={dataKey}
      onSelectionChange={(value, selectAll) => {
        if (selectAll !== true) {
          setSelectedSMStreams(value as SMStreamDto[]);
        }
      }}
      selectedItemsKey="selectSelectedSMStreamDtoItems"
      selectionMode="multiple"
      style={{ height: 'calc(100vh - 100px)' }}
    />
  );
};

export default memo(SMChannelSMStreamDataSelector);

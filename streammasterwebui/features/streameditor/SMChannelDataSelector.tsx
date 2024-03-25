import MinusButton from '@components/buttons/MinusButton';
import { useSMChannelLogoColumnConfig } from '@components/columns/useSMChannelLogoColumnConfig';
import useSMChannels from '@lib/smAPI/SMChannels/useSMChannels';
import { ColumnMeta } from '@components/dataSelector/DataSelectorTypes';
import { SMChannelDto } from '@lib/apiDefs';
import { GetMessage } from '@lib/common/common';
import { DeleteSMChannel } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { DeleteSMChannelRequest } from '@lib/smAPI/SMChannels/SMChannelsTypes';

import { ConfirmPopup, confirmPopup } from 'primereact/confirmpopup';
import { ProgressSpinner } from 'primereact/progressspinner';
import { v4 as uuidv4 } from 'uuid';
import { Suspense, lazy, memo, useCallback, useEffect, useMemo, useState } from 'react';
const DataSelector2 = lazy(() => import('@components/dataSelector/DataSelector2'));
const StreamCopyLinkDialog = lazy(() => import('@components/smstreams/StreamCopyLinkDialog'));
interface SMChannelDataSelectorProperties {
  readonly enableEdit?: boolean;
  readonly id: string;
  readonly reorderable?: boolean;
}

const SMChannelDataSelector = ({ enableEdit: propsEnableEdit, id, reorderable }: SMChannelDataSelectorProperties) => {
  const dataKey = `${id}-SMChannelDataSelector`;
  const { setSMChannelsIsLoading } = useSMChannels();
  const [enableEdit, setEnableEdit] = useState<boolean>(true);
  const { columnConfig: channelLogoColumnConfig } = useSMChannelLogoColumnConfig({ enableEdit });

  useEffect(() => {
    if (propsEnableEdit !== enableEdit) {
      setEnableEdit(propsEnableEdit ?? true);
    }
  }, [enableEdit, propsEnableEdit]);

  const actionBodyTemplate = useCallback((data: SMChannelDto) => {
    const accept = () => {
      setSMChannelsIsLoading(true);
      DeleteSMChannel({ smChannelId: data.id } as DeleteSMChannelRequest)
        .then((response) => {})
        .catch((error) => {
          console.error('Remove Channel', error.message);
        });
    };

    const reject = () => {};

    const confirm = (event: any) => {
      confirmPopup({
        target: event.currentTarget,
        message: 'Deletes "' + data.name + '" ?',
        icon: 'pi pi-exclamation-triangle',
        defaultFocus: 'accept',
        accept,
        reject
      });
    };

    return (
      <div className="flex p-0 justify-content-end align-items-center">
        <Suspense fallback={<ProgressSpinner>Loading...</ProgressSpinner>}>
          <StreamCopyLinkDialog realUrl={data?.realUrl} />
          <MinusButton iconFilled={false} onClick={confirm} tooltip="Remove Channel" />
        </Suspense>
      </div>
    );
  }, []);

  const columns = useMemo(
    (): ColumnMeta[] => [
      { field: 'channelNumber', width: '4rem' },
      channelLogoColumnConfig,
      // { field: 'logo', fieldType: 'image', width: '4rem' },
      { field: 'name', filter: true, sortable: true },
      { field: 'group', filter: true, sortable: true, width: '5rem' },
      { align: 'right', bodyTemplate: actionBodyTemplate, field: 'actions', fieldType: 'actions', header: 'Actions', width: '5rem' }
    ],
    [actionBodyTemplate, channelLogoColumnConfig]
  );

  return (
    <Suspense fallback={<div>Loading...</div>}>
      <ConfirmPopup />
      <DataSelector2
        selectRow
        showExpand
        columns={columns}
        defaultSortField="name"
        defaultSortOrder={1}
        emptyMessage="No Channels"
        headerName={GetMessage('channels').toUpperCase()}
        id={dataKey}
        queryFilter={useSMChannels}
        selectedSMStreamKey="SMChannelDataSelector"
        selectedSMChannelKey="SMChannelDataSelector"
        selectedItemsKey="selectSelectedSMChannelDtoItems"
        style={{ height: 'calc(100vh - 10px)' }}
      />
    </Suspense>
  );
};

export default memo(SMChannelDataSelector);

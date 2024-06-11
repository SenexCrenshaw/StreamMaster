import SMButton from '@components/sm/SMButton';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { AutoSetEPG, AutoSetEPGFromParameters } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { AutoSetEPGFromParametersRequest, AutoSetEPGRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import React from 'react';

interface AutoSetEPGSMChannelDialogProperties {
  readonly smChannel?: SMChannelDto;
}

const AutoSetEPGSMChannelDialog = ({ smChannel }: AutoSetEPGSMChannelDialogProperties) => {
  const selectedItemsKey = 'selectSelectedSMChannelDtoItems';
  const { queryFilter } = useQueryFilter('streameditor-SMChannelDataSelector');
  const { selectedItems } = useSelectedItems<SMChannelDto>(selectedItemsKey);
  const { selectAll } = useSelectAll('streameditor-SMChannelDataSelector');

  const ReturnToParent = React.useCallback(() => {}, []);

  const save = React.useCallback(async () => {
    if (selectedItems === undefined && smChannel === undefined && selectAll === true) {
      ReturnToParent();
      return;
    }

    if (selectAll === true) {
      if (!queryFilter) {
        ReturnToParent();
        return;
      }

      const request = {} as AutoSetEPGFromParametersRequest;
      request.Parameters = queryFilter;

      AutoSetEPGFromParameters(request)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        })
        .finally(() => {
          ReturnToParent();
        });

      return;
    }

    if (selectedItems.length === 0 && smChannel === undefined) {
      ReturnToParent();
      return;
    }

    const request = {
      Ids: smChannel === undefined ? selectedItems.map((item) => item.Id) : [smChannel.Id]
    } as AutoSetEPGRequest;

    await AutoSetEPG(request)
      .then(() => {})
      .catch((error) => {
        console.error('Set Stream Visibility Error: ', error.message);
        throw error;
      });
  }, [ReturnToParent, queryFilter, selectAll, selectedItems, smChannel]);

  return <SMButton icon="pi-book" buttonClassName="icon-blue" onClick={async () => save()} tooltip="Auto Set EPG" />;
};

AutoSetEPGSMChannelDialog.displayName = 'AutoSetEPGSMChannelDialog';

export default React.memo(AutoSetEPGSMChannelDialog);

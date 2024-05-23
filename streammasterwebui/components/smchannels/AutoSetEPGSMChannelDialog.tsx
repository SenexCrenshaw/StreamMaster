import OKButton from '@components/buttons/OKButton';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { AutoSetEPG, AutoSetEPGFromParameters } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { AutoSetEPGFromParametersRequest, AutoSetEPGRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import React, { useRef } from 'react';

interface AutoSetEPGSMChannelDialogProperties {
  readonly iconFilled?: boolean;
  readonly smChannel: SMChannelDto;
}

const AutoSetEPGSMChannelDialog = ({ iconFilled, smChannel }: AutoSetEPGSMChannelDialogProperties) => {
  const selectedItemsKey = 'selectSelectedSMChannelDtoItems';

  const smDialogRef = useRef<SMDialogRef>(null);
  const { queryFilter } = useQueryFilter('streameditor-SMChannelDataSelector');
  const { selectedItems } = useSelectedItems<SMChannelDto>(selectedItemsKey);
  const { selectAll } = useSelectAll('streameditor-SMChannelDataSelector');

  const ReturnToParent = React.useCallback(() => {}, []);

  const save = React.useCallback(async () => {
    if (selectedItems === undefined && smChannel === undefined && selectAll == true) {
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
          smDialogRef.current?.close();
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

  return (
    <SMDialog
      header={<OKButton onClick={async () => await save()} />}
      iconFilled={iconFilled}
      ref={smDialogRef}
      label={iconFilled !== true ? 'Auto Set EPGs' : undefined}
      title="AUTO SET EPGs"
      onHide={() => ReturnToParent()}
      buttonClassName="icon-blue"
      icon="pi-book"
      info="General"
      widthSize={2}
    >
      <div className="flex justify-content-center">
        <div className="flex flex-column py-2">Auto Set EPGs?</div>
      </div>
    </SMDialog>
  );
};

AutoSetEPGSMChannelDialog.displayName = 'AutoSetEPGSMChannelDialog';

export default React.memo(AutoSetEPGSMChannelDialog);

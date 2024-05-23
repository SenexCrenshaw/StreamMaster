import OKButton from '@components/buttons/OKButton';
import SMDialog, { SMDialogRef } from '@components/sm/SMDialog';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { AutoSetEPGFromParameters } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { AutoSetEPGFromParametersRequest } from '@lib/smAPI/smapiTypes';
import React, { useRef } from 'react';

interface AutoSetEPGSMChannelDialogProperties {
  readonly iconFilled?: boolean;
}

const AutoSetEPGSMChannelDialog = ({ iconFilled }: AutoSetEPGSMChannelDialogProperties) => {
  const smDialogRef = useRef<SMDialogRef>(null);
  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const { queryFilter } = useQueryFilter('streameditor-SMChannelDataSelector');

  const save = React.useCallback(async () => {
    if (!selectedStreamGroup || !queryFilter) {
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
  }, [queryFilter, selectedStreamGroup]);

  const ReturnToParent = React.useCallback(() => {}, []);

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

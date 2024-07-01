import OKButton from '@components/buttons/OKButton';
import { SMDialogRef } from '@components/sm/SMDialog';
import SMPopUp from '@components/sm/SMPopUp';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { useSelectedStreamGroup } from '@lib/redux/hooks/selectedStreamGroup';
import { AutoSetSMChannelNumbers } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { AutoSetSMChannelNumbersRequest, SMChannelDto } from '@lib/smAPI/smapiTypes';
import React, { useMemo, useRef } from 'react';

interface AutoSetSMChannelNumbersDialogProperties {
  readonly disabled?: boolean;
}

const AutoSetSMChannelNumbersDialog = ({ disabled }: AutoSetSMChannelNumbersDialogProperties) => {
  const selectedItemsKey = 'selectSelectedSMChannelDtoItems';
  const smDialogRef = useRef<SMDialogRef>(null);
  const { selectedStreamGroup } = useSelectedStreamGroup('StreamGroup');
  const { queryFilter } = useQueryFilter('streameditor-SMChannelDataSelector');
  const { selectAll } = useSelectAll('streameditor-SMChannelDataSelector');
  const { selectedItems } = useSelectedItems<SMChannelDto>(selectedItemsKey);
  const onAutoChannelsSave = React.useCallback(async () => {
    if (!selectedStreamGroup || !queryFilter) {
      return;
    }

    const request = {} as AutoSetSMChannelNumbersRequest;
    request.Parameters = queryFilter;
    request.streamGroupId = selectedStreamGroup.Id;

    AutoSetSMChannelNumbers(request)
      .then(() => {})
      .catch((error) => {
        console.error(error);
      })
      .finally(() => {
        smDialogRef.current?.hide();
      });
  }, [selectedStreamGroup, queryFilter]);
  const getTotalCount = useMemo(() => selectedItems?.length ?? 0, [selectedItems]);

  const ReturnToParent = React.useCallback(() => {}, []);

  return (
    <SMPopUp
      header={<OKButton onClick={async () => await onAutoChannelsSave()} />}
      iconFilled
      label="Set Channel #s"
      title="Set Channel #s"
      info=""
      menu
      hasCloseButton={false}
      onCloseClick={() => ReturnToParent()}
      buttonClassName="icon-yellow"
      icon="pi-sort-numeric-up-alt"
      placement="bottom-end"
      contentWidthSize="2"
    >
      <div className="text-container sm-center-stuff">Auto Set ({selectAll ? 'All' : getTotalCount}) channels?</div>
    </SMPopUp>
  );
};

AutoSetSMChannelNumbersDialog.displayName = 'Auto Set Channel Numbers';
export default React.memo(AutoSetSMChannelNumbersDialog);

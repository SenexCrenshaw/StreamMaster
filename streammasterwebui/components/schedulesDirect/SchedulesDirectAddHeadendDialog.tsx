import SMPopUp from '@components/sm/SMPopUp';
import { AddLineup } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { AddLineupRequest, HeadendDto } from '@lib/smAPI/smapiTypes';

import { memo } from 'react';

interface SchedulesDirectAddHeadendDialogProperties {
  readonly value: HeadendDto;
  readonly buttonDisabled?: boolean;
}

const SchedulesDirectAddHeadendDialog = ({ buttonDisabled, value }: SchedulesDirectAddHeadendDialogProperties) => {
  const addHeadEnd = async () => {
    if (!value) {
      return;
    }

    console.log(value);

    const toSend: AddLineupRequest = { Lineup: value.Lineup };

    AddLineup(toSend)
      .then((result) => {
        console.log(result);
      })
      .catch((error) => {
        console.log(error);
      });
  };

  return (
    <SMPopUp
      buttonDisabled={buttonDisabled}
      title="Subscribe to Lineup"
      icon="pi-plus"
      buttonClassName="icon-green"
      onOkClick={addHeadEnd}
      tooltip="Subscribe to Lineup"
    >
      <div className="flex flex-column align-content-center justify-content-center align-items-center">
        <div>Are you sure?</div>
        <div>You can only do this 6 times per day</div>
      </div>
    </SMPopUp>
  );
};

SchedulesDirectAddHeadendDialog.displayName = 'SchedulesDirectAddHeadendDialog';

export default memo(SchedulesDirectAddHeadendDialog);

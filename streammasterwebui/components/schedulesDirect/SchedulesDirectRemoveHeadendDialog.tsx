import SMPopUp from '@components/sm/SMPopUp';
import { RemoveLineup } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { HeadendDto, RemoveLineupRequest } from '@lib/smAPI/smapiTypes';

import { memo } from 'react';

interface SchedulesDirectAddHeadendDialogProperties {
  readonly value: HeadendDto;
}

const SchedulesDirectAddHeadendDialog = ({ value }: SchedulesDirectAddHeadendDialogProperties) => {
  const removeHeadEnd = async () => {
    if (!value) {
      return;
    }

    console.log(value);

    const request: RemoveLineupRequest = { Lineup: value.Lineup };

    RemoveLineup(request)
      .then((result) => {
        console.log(result);
      })
      .catch((error) => {
        console.log(error);
      });
  };

  return (
    <SMPopUp title="Unsubscribe Lineup" icon="pi-minus" buttonClassName="icon-red" onOkClick={removeHeadEnd}>
      <div className="flex flex-column align-content-center justify-content-center align-items-center">
        <div>Are you sure?</div>
        <div>You can only do this 6 times per day</div>
      </div>
    </SMPopUp>
  );
};

SchedulesDirectAddHeadendDialog.displayName = 'SchedulesDirectAddHeadendDialog';

export default memo(SchedulesDirectAddHeadendDialog);

import AddButton from '@components/buttons/AddButton';
import { AddLineup } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { AddLineupRequest, HeadendDto } from '@lib/smAPI/smapiTypes';

import { memo } from 'react';

interface SchedulesDirectAddHeadendDialogProperties {
  readonly value: HeadendDto;
}

const SchedulesDirectAddHeadendDialog = ({ value }: SchedulesDirectAddHeadendDialogProperties) => {
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
    <div className="flex">
      <AddButton iconFilled={false} onClick={async () => await addHeadEnd()} />
    </div>
  );
};

SchedulesDirectAddHeadendDialog.displayName = 'SchedulesDirectAddHeadendDialog';

export default memo(SchedulesDirectAddHeadendDialog);

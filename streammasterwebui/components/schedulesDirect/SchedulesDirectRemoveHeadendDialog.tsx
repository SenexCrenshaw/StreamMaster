import MinusButton from '@components/buttons/MinusButton';
import { RemoveLineup } from '@lib/smAPI/SchedulesDirect/SchedulesDirectCommands';
import { HeadendDto, RemoveLineupRequest } from '@lib/smAPI/smapiTypes';

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
    <div className="flex">
      <MinusButton iconFilled={false} onClick={async () => await addHeadEnd()} />
    </div>
  );
};

SchedulesDirectAddHeadendDialog.displayName = 'SchedulesDirectAddHeadendDialog';

export default memo(SchedulesDirectAddHeadendDialog);

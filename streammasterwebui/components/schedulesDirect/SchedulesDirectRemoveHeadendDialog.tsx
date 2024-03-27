import MinusButton from '@components/buttons/MinusButton';
import { HeadendDto, SchedulesDirectRemoveLineupApiArg, useSchedulesDirectRemoveLineupMutation } from '@lib/iptvApi';
import { memo } from 'react';

interface SchedulesDirectAddHeadendDialogProperties {
  readonly value: HeadendDto;
}

const SchedulesDirectAddHeadendDialog = ({ value }: SchedulesDirectAddHeadendDialogProperties) => {
  const [schedulesDirectRemoveLineupMutation] = useSchedulesDirectRemoveLineupMutation();

  const addHeadEnd = async () => {
    if (!value) {
      return;
    }

    console.log(value);

    const toSend: SchedulesDirectRemoveLineupApiArg = { lineup: value.lineup };

    schedulesDirectRemoveLineupMutation(toSend)
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

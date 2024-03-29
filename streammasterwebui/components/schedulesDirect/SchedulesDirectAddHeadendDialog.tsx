import AddButton from '@components/buttons/AddButton';

import { memo } from 'react';

interface SchedulesDirectAddHeadendDialogProperties {
  readonly value: HeadendDto;
}

const SchedulesDirectAddHeadendDialog = ({ value }: SchedulesDirectAddHeadendDialogProperties) => {
  const [schedulesDirectAddLineupMutation] = useSchedulesDirectAddLineupMutation();

  const addHeadEnd = async () => {
    if (!value) {
      return;
    }

    console.log(value);

    const toSend: SchedulesDirectAddLineupApiArg = { lineup: value.lineup };

    schedulesDirectAddLineupMutation(toSend)
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

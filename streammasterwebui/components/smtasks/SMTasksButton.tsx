import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import useGetSMTasks from '@lib/smAPI/SMTasks/useGetSMTasks';
import { memo, useMemo, useRef } from 'react';
import SMTasksDataSelector from './SMTasksDataSelector';

const SMTasksButton = () => {
  const { isLoading, data } = useGetSMTasks();

  const popUpRef = useRef<SMPopUpRef>(null);

  const isRunning = useMemo((): boolean => {
    if (isLoading || !data) {
      return false;
    }
    return data.some((task) => task.IsRunning);
  }, [data, isLoading]);

  return (
    <SMPopUp
      modal
      ref={popUpRef}
      buttonClassName={isRunning ? 'icon-green' : 'icon-blue'}
      contentWidthSize="5"
      icon="pi-list-check"
      iconFilled
      info=""
      placement="bottom-end"
      title="Tasks"
      showRemember={false}
    >
      <SMTasksDataSelector />
    </SMPopUp>
  );
};

SMTasksButton.displayName = 'SMTasksButton';

export default memo(SMTasksButton);

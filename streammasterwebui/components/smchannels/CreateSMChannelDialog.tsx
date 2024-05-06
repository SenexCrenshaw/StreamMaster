import React from 'react';
import SMChannelDialog from './SMChannelDialog';

interface CopySMChannelProperties {}

const CreateSMChannelDialog = () => {
  const onSave = React.useCallback(async () => {}, []);

  return <SMChannelDialog />;
};

CreateSMChannelDialog.displayName = 'CreateSMChannelDialog';

export default React.memo(CreateSMChannelDialog);

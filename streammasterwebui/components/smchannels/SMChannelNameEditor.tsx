import StringEditorBodyTemplate from '@components/inputs/StringEditorBodyTemplate';
import { SetSMChannelName } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelNameRequest } from '@lib/smAPI/smapiTypes';
import React from 'react';

export interface SMChannelNameEditorProperties {
  readonly data: SMChannelDto;
  readonly onClick?: () => void;
}

const SMChannelNameEditor = ({ data, onClick }: SMChannelNameEditorProperties) => {
  const onUpdateM3UStream = React.useCallback(
    async (name: string) => {
      if (data.id === 0 || !name || name === '' || data.name === name) {
        return;
      }

      const toSend = { smChannelId: data.id, name: name } as SetSMChannelNameRequest;

      await SetSMChannelName(toSend)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        });
    },
    [data.id, data.name]
  );

  if (data.name === undefined) {
    return <span className="sm-inputtext" />;
  }

  return (
    <StringEditorBodyTemplate
      onClick={onClick}
      onChange={async (e) => {
        if (e !== undefined) {
          await onUpdateM3UStream(e);
        }
      }}
      value={data.name}
    />
  );
};

SMChannelNameEditor.displayName = 'Channel Number Editor';

export default React.memo(SMChannelNameEditor);

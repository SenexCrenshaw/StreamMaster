import StringEditor from '@components/inputs/StringEditor';
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
      if (data.Id === 0 || !name || name === '' || data.Name === name) {
        return;
      }

      const toSend = { Name: name, SMChannelId: data.Id } as SetSMChannelNameRequest;

      await SetSMChannelName(toSend)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        });
    },
    [data.Id, data.Name]
  );

  if (data.Name === undefined) {
    return <span className="sm-inputtext" />;
  }

  return (
    <StringEditor
      showClear
      darkBackGround={false}
      onClick={onClick}
      onSave={async (e) => {
        if (e !== undefined) {
          await onUpdateM3UStream(e);
        }
      }}
      value={data.Name}
    />
  );
};

SMChannelNameEditor.displayName = 'Channel Name Editor';

export default React.memo(SMChannelNameEditor);

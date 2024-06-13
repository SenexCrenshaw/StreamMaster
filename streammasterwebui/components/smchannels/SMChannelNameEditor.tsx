import StringEditor from '@components/inputs/StringEditor';
import { useIsTrue } from '@lib/redux/hooks/isTrue';
import { SetSMChannelName } from '@lib/smAPI/SMChannels/SMChannelsCommands';
import { SMChannelDto, SetSMChannelNameRequest } from '@lib/smAPI/smapiTypes';
import React from 'react';

export interface SMChannelNameEditorProperties {
  readonly smChannelDto: SMChannelDto;
  readonly onClick?: () => void;
}

const SMChannelNameEditor = ({ smChannelDto, onClick }: SMChannelNameEditorProperties) => {
  const { isTrue: isNameLoading, setIsTrue: setIsNameLoading } = useIsTrue('SMChannelDataSelectorIsNameLoading');

  const onUpdateM3UStream = React.useCallback(
    async (name: string) => {
      if (smChannelDto.Id === 0 || !name || name === '' || smChannelDto.Name === name) {
        return;
      }
      setIsNameLoading(true);
      const toSend = { Name: name, SMChannelId: smChannelDto.Id } as SetSMChannelNameRequest;

      await SetSMChannelName(toSend)
        .then(() => {})
        .catch((error) => {
          console.error(error);
        })
        .finally(() => {
          setIsNameLoading(false);
        });
    },
    [smChannelDto, setIsNameLoading]
  );

  if (smChannelDto.Name === undefined) {
    return <span className="sm-inputtext" />;
  }

  return (
    <StringEditor
      showClear
      darkBackGround={false}
      isLoading={isNameLoading}
      onClick={onClick}
      onSave={async (e) => {
        if (e !== undefined) {
          await onUpdateM3UStream(e);
        }
      }}
      value={smChannelDto.Name}
    />
  );
};

SMChannelNameEditor.displayName = 'Channel Name Editor';

export default React.memo(SMChannelNameEditor);

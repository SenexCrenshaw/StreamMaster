import SMPopUp, { SMPopUpRef } from '@components/sm/SMPopUp';
import { Logger } from '@lib/common/logger';
import { RemoveCommandProfile } from '@lib/smAPI/Profiles/ProfilesCommands';
import { CommandProfileDto, RemoveCommandProfileRequest } from '@lib/smAPI/smapiTypes';
import { useCallback, useRef } from 'react';

interface RemoveCommandProfileDialogProps {
  videoOutputProfileDto: CommandProfileDto;
}

const RemoveCommandProfileDialog = ({ ...props }: RemoveCommandProfileDialogProps) => {
  const smPopUpRef = useRef<SMPopUpRef>(null);
  const remove = useCallback(() => {
    const request = {
      ProfileName: props.videoOutputProfileDto.ProfileName
    } as RemoveCommandProfileRequest;

    RemoveCommandProfile(request)
      .then((response) => {})
      .catch((error) => {
        Logger.error('RemoveCommandProfileDialog', { error });
      })
      .finally(() => {
        smPopUpRef.current?.hide();
      });
  }, [props]);

  return (
    <SMPopUp
      contentWidthSize="2"
      buttonDisabled={
        props.videoOutputProfileDto.IsReadOnly ||
        props.videoOutputProfileDto.ProfileName.toLowerCase() === 'default' ||
        props.videoOutputProfileDto.ProfileName.toLowerCase() === 'defaultffmpeg' ||
        props.videoOutputProfileDto.ProfileName.toLowerCase() === 'streammaster'
      }
      buttonClassName="icon-red"
      icon="pi-times"
      info=""
      title="Remove Profile?"
      modal
      modalCentered
      onOkClick={() => {
        remove();
      }}
      ref={smPopUpRef}
      okButtonDisabled={!props.videoOutputProfileDto.ProfileName}
      tooltip="Remove Profile"
      zIndex={10}
    >
      <div className="sm-center-stuff">
        <div className="text-container"> {props.videoOutputProfileDto.ProfileName}</div>
      </div>
    </SMPopUp>
  );
};

export default RemoveCommandProfileDialog;

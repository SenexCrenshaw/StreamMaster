import AddButton from '@components/buttons/AddButton';
import { SMChannelDto } from '@lib/smAPI/smapiTypes';
import { memo } from 'react';

interface EPGSGEditorProperties {
  readonly data: SMChannelDto;
  readonly enableEditMode?: boolean;
}

const EPGSGEditor = ({ data, enableEditMode }: EPGSGEditorProperties) => {
  // const onUpdateVideoStream = async (epg: string) => {
  //   if (!data.Id) {
  //     return;
  //   }

  //   const request = {} as SetSMChannelEPGIdRequest;
  //   request.SMChannelId = data.Id;
  //   request.EPGId = epg;

  //   await SetSMChannelEPGId(request)
  //     .then(() => {})
  //     .catch((error) => {
  //       console.error(error);
  //     });
  // };

  return (
    <div className="flex justify-content-center align-items-center">
      <AddButton onClick={(e) => console.log('click')} />
    </div>
  );
};

export default memo(EPGSGEditor);

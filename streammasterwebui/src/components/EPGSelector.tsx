import * as React from 'react';

import { Toast } from 'primereact/toast';
import { classNames } from 'primereact/utils';
import DropDownEditorBodyTemplate from './DropDownEditorBodyTemplate';
import { type ProgrammeNameDto, type UpdateVideoStreamRequest, type VideoStreamDto } from '../store/iptvApi';
import { useProgrammesGetProgrammeNamesQuery, useVideoStreamsUpdateVideoStreamMutation } from '../store/iptvApi';

const EPGSelector = (props: EPGSelectorProps) => {
  const toast = React.useRef<Toast>(null);

  const [programme, setProgramme] = React.useState<string>('');
  const [channel, setChannel] = React.useState<string>('');
  const [dataDataSource, setDataSource] = React.useState<ProgrammeNameDto[]>([]);

  const programmeNamesQuery = useProgrammesGetProgrammeNamesQuery();

  const [videoStreamsUpdateVideoStreamMutation] = useVideoStreamsUpdateVideoStreamMutation();

  React.useEffect(() => {
    if (!programmeNamesQuery.data) {
      return;
    }

    const newData = [...programmeNamesQuery.data];
    newData.unshift({ displayName: '<Blank>' });
    setDataSource(newData);

    return () => {
      setDataSource([]);
    };

  }, [programmeNamesQuery.data]);

  React.useEffect(() => {
    if (!dataDataSource) {
      return;
    }

    if (props.value === null || props.value === undefined) {
      setProgramme('<Blank>');
      setChannel('<Blank>');
      return;
    }

    if (programmeNamesQuery.data === undefined) {
      return;
    }

    const test = programmeNamesQuery.data.find((a) => a.channel === props.value);
    if (test === undefined || test.displayName === undefined || test.channel === undefined) {
      return;
    }

    setProgramme(test.displayName);
    setChannel(test.channel);

  }, [channel, dataDataSource, programmeNamesQuery.data, props.value]);

  const onEPGChange = React.useCallback(async (displayName: string) => {

    if (programmeNamesQuery.data === undefined || props.data === undefined || props.data.id === '') {
      return;
    }

    let toChange = '';

    if (displayName !== '<Blank>') {
      const test = programmeNamesQuery.data.find((a) => a.displayName === displayName);
      if (test === undefined || test.displayName === undefined || test.channel === undefined) {
        return;
      }

      if (props.data.user_Tvg_ID === test.channel) {
        return;
      }

      setProgramme(test.displayName);
      setChannel(test.channel);
      toChange = test.channel;
    }

    if (props.onChange) {
      props.onChange(channel);
    }


    console.debug(channel);
    const data = {} as UpdateVideoStreamRequest;
    data.tvg_ID = toChange;
    data.id = props.data.id;


    await videoStreamsUpdateVideoStreamMutation(data)
      .then(() => {
        if (toast.current) {

          toast.current.show({
            detail: `Updated Stream`,
            life: 3000,
            severity: 'success',
            summary: 'Successful',
          });

        }
      }).catch(() => {
        if (toast.current) {
          toast.current.show({
            detail: `Update Stream Failed`,
            life: 3000,
            severity: 'error',
            summary: 'Error',
          });
        }
      });



  }, [channel, programmeNamesQuery.data, props, videoStreamsUpdateVideoStreamMutation]);

  const className = classNames('iconSelector p-0 m-0 w-full z-5 ', props.className);

  if (props.enableEditMode !== true) {

    return (
      <div className='flex h-full justify-content-center align-items-center p-0 m-0'>
        {programme !== '' ? programme : 'Dummy'}
      </div>
    )

  }

  return (
    <>
      <Toast position="bottom-right" ref={toast} />
      <div className="iconSelector flex w-full justify-content-center align-items-center">

        <DropDownEditorBodyTemplate
          className={className}
          data={dataDataSource.map((a) => (a.displayName ?? ''))}
          onChange={async (e) => {
            await onEPGChange(e);
          }}
          value={programme}
        />

      </div>
    </>
  );
};

EPGSelector.displayName = 'EPGSelector';
EPGSelector.defaultProps = {
  className: null,
  enableEditMode: true,
  onChange: null,
  value: null,
};

type EPGSelectorProps = {
  className?: string | null;
  data?: VideoStreamDto | undefined;
  enableEditMode?: boolean;
  onChange?: ((value: string) => void) | null;
  value?: string | null;
};

export default React.memo(EPGSelector);

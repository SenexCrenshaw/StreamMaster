import * as React from 'react';

import { classNames } from 'primereact/utils';
import DropDownEditorBodyTemplate from './DropDownEditorBodyTemplate';
import { type UpdateVideoStreamRequest } from '../store/iptvApi';
import { type ProgrammeNameDto, type VideoStreamDto } from '../store/iptvApi';
import { useProgrammesGetProgrammeNamesQuery, useVideoStreamsUpdateVideoStreamMutation } from '../store/iptvApi';

const EPGSelector = (props: EPGSelectorProps) => {

  const [programme, setProgramme] = React.useState<string>('');
  const [dataDataSource, setDataSource] = React.useState<ProgrammeNameDto[]>([]);
  const programmeNamesQuery = useProgrammesGetProgrammeNamesQuery();

  const [videoStreamsUpdateVideoStreamMutation] = useVideoStreamsUpdateVideoStreamMutation();

  React.useEffect(() => {
    if (!programmeNamesQuery.data) {
      return;
    }

    console.debug('EPGSelector', programmeNamesQuery.data.length);
    const newData = [...programmeNamesQuery.data];
    newData.unshift({ displayName: '<Blank>' });
    console.debug('EPGSelector', newData.length);

    setDataSource(newData);


  }, [programmeNamesQuery.data]);

  React.useEffect(() => {
    if (!dataDataSource) {
      return;
    }

    if (props.value === null || props.value === undefined) {
      setProgramme('<Blank>');
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

  }, [dataDataSource, programmeNamesQuery.data, props.value]);

  const onEPGChange = React.useCallback(async (displayName: string) => {

    if (props.data === undefined || props.data.id === '') {
      props.onChange?.(displayName);
      return;
    }

    const data = {} as UpdateVideoStreamRequest;
    data.tvg_ID = displayName;
    data.id = props.data.id;

    await videoStreamsUpdateVideoStreamMutation(data)
      .then(() => { props.onChange?.(displayName) }).catch(() => { }).finally(() => { });

  }, [props, videoStreamsUpdateVideoStreamMutation]);

  const className = classNames('iconSelector p-0 m-0 w-full z-5 ', props.className);

  if (props.enableEditMode !== true) {

    return (
      <div className='flex h-full justify-content-center align-items-center p-0 m-0'>
        {programme !== '' ? programme : 'Dummy'}
      </div>
    )
  }

  return (
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
  );
};

EPGSelector.displayName = 'EPGSelector';
EPGSelector.defaultProps = {
  className: null,
  enableEditMode: true,
  value: null,
};

type EPGSelectorProps = {
  className?: string;
  data?: VideoStreamDto;
  enableEditMode?: boolean;
  onChange?: ((value: string) => void);
  value?: string;
};

export default React.memo(EPGSelector);

import useGetChannelGroups from '@lib/smAPI/ChannelGroups/useGetChannelGroups';
import { ChannelGroupDto, SMChannelDto } from '@lib/smAPI/smapiTypes';
import { ReactNode, forwardRef, memo, useEffect, useMemo, useRef, useState } from 'react';

import SMDropDown, { SMDropDownRef } from '@components/sm/SMDropDown';

interface SMChannelGroupDropDownProperties {
  readonly smChannel?: SMChannelDto;
  readonly value?: string;
  readonly darkBackGround?: boolean;
  readonly fixed?: boolean;
  readonly label?: string;
  readonly labelInline?: boolean;
  readonly onChange: (value: string) => void;
}

const SMChannelGroupDropDown = forwardRef<SMDropDownRef, SMChannelGroupDropDownProperties>((props: SMChannelGroupDropDownProperties, ref) => {
  const { darkBackGround, value, smChannel, fixed = false, onChange, label, labelInline = false } = props;
  const smDropownRef = useRef<SMDropDownRef>(null);
  const { data } = useGetChannelGroups();
  const [channelGroup, setChannelGroup] = useState<ChannelGroupDto | null>(null);

  useEffect(() => {
    if (!data) {
      return;
    }

    if (smChannel !== undefined && (channelGroup === null || channelGroup.Name !== smChannel.Group)) {
      const found = data.find((predicate) => predicate.Name === smChannel.Group);
      if (found) setChannelGroup(found);
      return;
    }

    if (value !== undefined && (channelGroup === null || channelGroup.Name !== value)) {
      const found = data.find((predicate) => predicate.Name === value);
      if (found) setChannelGroup(found);
      return;
    }
  }, [channelGroup, data, value, smChannel]);

  const itemTemplate = (option: ChannelGroupDto) => {
    if (option === undefined) {
      return null;
    }

    return <div className="text-container">{option.Name}</div>;
  };

  const buttonTemplate = useMemo((): ReactNode => {
    if (!channelGroup) {
      return <div className="text-xs text-container text-white-alpha-40 pl-1">None</div>;
    }

    return (
      <div className="sm-epg-selector">
        <div className="text-container pl-1">{channelGroup.Name}</div>
      </div>
    );
  }, [channelGroup]);

  const getDiv = useMemo(() => {
    let ret = 'stringeditor';

    if (label && !labelInline) {
      ret += '';
    }

    if (labelInline) {
      ret += ' align-items-start';
    } else {
      ret += ' align-items-center';
    }

    return ret;
  }, [label, labelInline]);

  return (
    <>
      {label && !labelInline && (
        <>
          <label className="pl-15">{label.toUpperCase()}</label>
        </>
      )}
      <div className={getDiv}>
        {label && labelInline && <div className={labelInline ? 'w-4' : 'w-6'}>{label.toUpperCase()}</div>}
        <SMDropDown
          buttonLabel="GROUP"
          buttonDarkBackground={darkBackGround}
          buttonTemplate={buttonTemplate}
          closeOnSelection
          data={data}
          dataKey="Name"
          filter
          filterBy="Name"
          fixed={fixed}
          itemTemplate={itemTemplate}
          onChange={(e) => {
            onChange(e.Name);
          }}
          ref={smDropownRef}
          title="GROUP"
          value={channelGroup}
          contentWidthSize="2"
          zIndex={10}
        />
      </div>
    </>
  );
});

export default memo(SMChannelGroupDropDown);

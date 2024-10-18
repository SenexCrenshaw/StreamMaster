import SMDropDown from '@components/sm/SMDropDown';
import { getIconUrl } from '@lib/common/common';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { ChannelGroupDto, NameLogo } from '@lib/smAPI/smapiTypes';
import useGetSMChannelNameLogos from '@lib/smAPI/SMChannels/useGetSMChannelNameLogos';

import React, { useCallback } from 'react';

interface SMChannelSelectorProperties {
  readonly onChange?: (value: string) => void;
}

const SMChannelSelector: React.FC<SMChannelSelectorProperties> = ({ onChange }) => {
  const dataKey = 'smChannelSelector';
  const { data } = useGetSMChannelNameLogos();
  const { selectedItems } = useSelectedItems<ChannelGroupDto>(dataKey);
  const { selectAll } = useSelectAll(dataKey);

  const itemTemplate = useCallback((row: NameLogo): JSX.Element => {
    if (row === undefined) {
      return <div />;
    }
    const iconUrl = row.Logo ? getIconUrl(row.Logo, '/images/default.png', false, null) : '/images/default.png';

    return (
      <div className="flex sm-start-stuff sm-w-10">
        <div className="icon-button-template">
          <img className="icon-button-template" alt="Icon logo" src={iconUrl} />
        </div>
        <div className="pl-1 text-xs text-container"> {row.Name}</div>
      </div>
    );
  }, []);

  return (
    <SMDropDown
      buttonLabel="Channels"
      // buttonTemplate={buttonTemplate}
      closeOnSelection={false}
      data={data}
      filter
      filterBy="Name"
      itemTemplate={itemTemplate}
      onChange={async (e: any) => {
        // await onIChange(e.value);
      }}
      title="Channels"
      // propertyToMatch="Name"
      select
      selectedItemsKey={dataKey}
      contentWidthSize="2"
    />
  );
};

SMChannelSelector.displayName = 'Stream Proxy Type Dropdown';

export default React.memo(SMChannelSelector);

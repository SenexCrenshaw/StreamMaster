import { useDispatch, useSelector } from 'react-redux';
import { type ChannelGroupDto } from '../../store/iptvApi';
import { type AppDispatch, type RootState } from '../store';
import { makeselectedChannelGroups, setselectedChannelGroupsInternal } from './selectedChannelGroupsSlice';

export const useSelectedChannelGroups = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  // Use the memoized selector
  const selectedChannelGroups = useSelector((state: RootState) => makeselectedChannelGroups(state, typename));

  const setSelectedChannelGroups = (ChannelGroupDtos: ChannelGroupDto[]) => {
    if (typename === undefined) {
      return;
    }

    dispatch(setselectedChannelGroupsInternal({
      ChannelGroupDtos,
      typename,
    }));
  };

  return { selectedChannelGroups, setSelectedChannelGroups };
};


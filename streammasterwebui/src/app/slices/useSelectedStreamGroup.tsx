import { useDispatch, useSelector } from 'react-redux';
import { type AppDispatch, type RootState } from '../store';
import { setSelectedStreamGroupInternal } from './selectedStreamGroupSlice';
import { type StreamGroupDto } from '../../store/iptvApi';

export const useSelectedStreamGroup = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setSelectedStreamGroup = (streamGroup: StreamGroupDto) => {
    dispatch(setSelectedStreamGroupInternal({
      streamGroup: streamGroup,
      typename
    }));
  };

  const selectedStreamGroup = useSelector((rootState: RootState) => rootState.selectedStreamGroup[typename]);

  return { selectedStreamGroup, setSelectedStreamGroup };
};

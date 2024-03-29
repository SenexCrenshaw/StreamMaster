import { useDispatch, useSelector } from 'react-redux';
import { type AppDispatch, type RootState } from '../store';

import { setSelectedStreamGroupInternal } from './selectedStreamGroupSlice';

export const useSelectedStreamGroup = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setSelectedStreamGroup = (streamGroup: StreamGroupDto | undefined): void => {
    if (streamGroup === undefined) {
      dispatch(
        setSelectedStreamGroupInternal({
          streamGroup: {} as StreamGroupDto,
          typename
        })
      );
    } else {
      dispatch(
        setSelectedStreamGroupInternal({
          streamGroup,
          typename
        })
      );
    }
  };

  const selectedStreamGroup = useSelector((rootState: RootState) => rootState.selectedStreamGroup[typename]);

  return { selectedStreamGroup, setSelectedStreamGroup };
};

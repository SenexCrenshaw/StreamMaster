import { useDispatch, useSelector } from 'react-redux';
import { type AppDispatch, type RootState } from '../store';
import { setStreamGroupToRemove as setStreamGroupToRemoveInternal } from './streamGroupToRemoveSlice';


export const useStreamGroupToRemove = (typename: string) => {
  const dispatch: AppDispatch = useDispatch();

  const setStreamGroupToRemove = (toRemove: number) => {
    dispatch(setStreamGroupToRemoveInternal({
      toRemove: toRemove,
      typename
    }));
  };

  const streamGroupToRemove = useSelector((rootState: RootState) => rootState.streamGroupToRemove[typename]);

  return { setStreamGroupToRemove, streamGroupToRemove };
};

import { coreReducer, initialState } from './core.reducer';

describe('Core Reducer', () => {
  describe('an unknown action', () => {
    it('should return the previous state', () => {
      const action = {} as any;

      const result = coreReducer(initialState, action);

      expect(result).toBe(initialState);
    });
  });
});
